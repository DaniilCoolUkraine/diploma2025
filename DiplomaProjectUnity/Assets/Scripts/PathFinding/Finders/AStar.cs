using System.Collections.Generic;
using System.Diagnostics;
using DiplomaProject.General;
using DiplomaProject.PathFinding.Utils;
using DiplomaProject.TileMap;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DiplomaProject.PathFinding.Finders
{
    public class AStar : MonoBehaviour
    { 
        [SerializeField] private int _length;

        public void FindPath(int2 startPosition, int2 endPosition, List<TileNode> pathNodeArray, int2 gridSize)
        {
            var jobs = new NativeArray<JobHandle>(_length, Allocator.TempJob);

            for (int i = 0; i < _length; i++)
            {
                var job = new FindPathJob()
                {
                    startPosition = startPosition,
                    endPosition = endPosition,
                    pathNodeArray = SetupGrid(endPosition, gridSize, pathNodeArray),
                    gridSize = gridSize
                };

                jobs[i] = job.Schedule();
            }

            JobHandle.CompleteAll(jobs);
            jobs.Dispose();
        }
        
        private NativeArray<PathNode> SetupGrid(int2 endPosition, int2 gridSize, List<TileNode> nodes)
        {
            var pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.TempJob);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var pathNode = new PathNode();
                    pathNode.X = x;
                    pathNode.Y = y;

                    pathNode.Index = TileMapUtils.CalculateIndex(x, y, gridSize.x);
                    pathNode.PreviousIndex = PathFinderConstants.INVALID_PREVIOUS_INDEX;

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = TileMapUtils.CalculateDistanceCost(new int2(x, y), endPosition);
                    pathNode.CalculateFCost();

                    pathNode.Walkable = nodes[pathNode.Index].IsWalkable;

                    pathNodeArray[pathNode.Index] = pathNode;
                }
            }

            return pathNodeArray;
        }
    }

    [BurstCompile]
    public struct FindPathJob : IJob
    {
        public int2 gridSize;
        [DeallocateOnJobCompletion]
        public NativeArray<PathNode> pathNodeArray;

        public int2 startPosition;
        public int2 endPosition;

        public void Execute()
        {
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            NativeArray<int2> neighbourOffsetsArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetsArray[0] = new int2(1, 0); // Right
            neighbourOffsetsArray[1] = new int2(-1, 0); // Left
            neighbourOffsetsArray[2] = new int2(0, 1); // Up
            neighbourOffsetsArray[3] = new int2(0, -1); // Down
            neighbourOffsetsArray[4] = new int2(1, 1); // Right-Up
            neighbourOffsetsArray[5] = new int2(-1, 1); // Left-Up
            neighbourOffsetsArray[6] = new int2(1, -1); // Right-Down
            neighbourOffsetsArray[7] = new int2(-1, -1); // Left-Down

            // get start and end nodes
            var startNode = pathNodeArray[TileMapUtils.CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();

            var endNode = pathNodeArray[TileMapUtils.CalculateIndex(endPosition.x, endPosition.y, gridSize.x)];

            pathNodeArray[startNode.Index] = startNode;
            
            openList.Add(startNode.Index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCost(openList, pathNodeArray);
                var currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNode.Index)
                {
                    break;
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }
                
                closedList.Add(currentNodeIndex);
                for (int i = 0; i < neighbourOffsetsArray.Length; i++)
                {
                    var neighbourOffset = neighbourOffsetsArray[i];
                    var neighbourPosition = new int2(currentNode.X + neighbourOffset.x, currentNode.Y + neighbourOffset.y);

                    if (!TileMapUtils.TilePositionIsInBounds(neighbourPosition.x, neighbourPosition.y, gridSize.x, gridSize.y))
                    {
                        continue;
                    }

                    int neighbourNodeIndex = TileMapUtils.CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                    if (closedList.Contains(neighbourNodeIndex))
                    {
                        continue;
                    }

                    var neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.Walkable)
                    {
                        continue;
                    }
                    
                    int tentativeGCost = currentNode.gCost + TileMapUtils.CalculateDistanceCost(new int2(currentNode.X, currentNode.Y), neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.PreviousIndex = currentNode.Index;
                        
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.Index))
                        {
                            openList.Add(neighbourNodeIndex);
                        }
                    }
                }
            }

            endNode = pathNodeArray[endNode.Index];

            if (endNode.PreviousIndex == PathFinderConstants.INVALID_PREVIOUS_INDEX)
            {
                Debug.LogError("path doesnt exist");
            }
            else
            {
                var path = CollectPath(pathNodeArray, endNode);
                path.Dispose();
            }

            openList.Dispose();
            closedList.Dispose();
            neighbourOffsetsArray.Dispose();
        }
        
        private int GetLowestFCost(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            var lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++)
            {
                var testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }

            return lowestCostPathNode.Index;
        }

        private NativeList<int2> CollectPath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
        {
            var path = new NativeList<int2>(Allocator.Temp);
            
            if (endNode.PreviousIndex != PathFinderConstants.INVALID_PREVIOUS_INDEX)
            {
                path.Add(new int2(endNode.X, endNode.Y));
                
                var currentNode = endNode;

                while (currentNode.PreviousIndex != PathFinderConstants.INVALID_PREVIOUS_INDEX)
                {
                    var cameFromNode = pathNodeArray[currentNode.PreviousIndex];
                    path.Add(new int2(cameFromNode.X, cameFromNode.Y));
                    currentNode = cameFromNode;
                }
            }
            
            return path;
        }
    }
}