using DiplomaProject.TileMap;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.PathFinding
{
    public class Astar
    {
        private const int NODE_INVALID_INDEX = -1;
        private const int MOVE_DIAGONAL_COST = 14;
        private const int MOVE_STRAIGHT_COST = 10;

        private int2 _gridSize;
        private NativeArray<PathNode> _pathNodesArray;

        public void CreateGrid(Vector2Int gridSize)
        {
            _gridSize = new int2(gridSize.x, gridSize.y);
        }

        public void FindPath(Vector2Int startPoint, Vector2Int endPoint)
        {
            var point = new int2(endPoint.x, endPoint.y);

            InitializeArray(point);
            FindPath(new int2(startPoint.x, startPoint.y), point);

            _pathNodesArray.Dispose();
        }

        private void InitializeArray(int2 endPoint)
        {
            _pathNodesArray = new NativeArray<PathNode>(_gridSize.x * _gridSize.y, Allocator.Temp);

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    PathNode node = new PathNode();

                    node.x = x;
                    node.y = y;

                    node.index = TileMapUtils.CalculateIndex(x, y, _gridSize.x);
                    node.previousIndex = NODE_INVALID_INDEX;

                    node.gCost = int.MaxValue;
                    node.hCost = CalculateDistanceCost(new int2(x, y), endPoint);
                    node.CalculateFCost();

                    node.walkable = true;

                    _pathNodesArray[node.index] = node;
                }
            }
        }

        private void FindPath(int2 startPoint, int2 endPoint)
        {
            NativeList<int> openNodes = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedNodes = new NativeList<int>(Allocator.Temp);

            var offsets = GetOffsets();

            int startNodeIndex = TileMapUtils.CalculateIndex(startPoint.x, startPoint.y, _gridSize.x);
            int endNodeIndex = TileMapUtils.CalculateIndex(endPoint.x, endPoint.y, _gridSize.x);
            
            var startNode = _pathNodesArray[startNodeIndex];
            startNode.gCost = 0;
            startNode.CalculateFCost();

            _pathNodesArray[startNodeIndex] = startNode;
            openNodes.Add(startNode.index);

            while (openNodes.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openNodes, _pathNodesArray);
                var currentNode = _pathNodesArray[currentNodeIndex];
                int2 currentNodePosition = new int2(currentNode.x, currentNode.y); 

                Debug.Log($"checking node ({currentNode.x}, {currentNode.y}), index {currentNodeIndex}");
                
                if (currentNodeIndex == endNodeIndex)
                {
                    break;
                }

                if (openNodes.Contains(currentNodeIndex))
                {
                    openNodes.RemoveAtSwapBack(openNodes.IndexOf(currentNodeIndex));
                    break;
                }

                closedNodes.Add(currentNodeIndex);

                for (int i = 0; i < offsets.Length; i++)
                {
                    int2 offset = offsets[i];
                    int2 neighbourPosition = new int2(currentNode.x + offset.x, currentNode.y + offset.y);

                    Debug.Log($"checking neighbour at offset ({offset.x}, {offset.y})");

                    if (!NeighbourIsValid(neighbourPosition, closedNodes))
                    {
                        Debug.Log("neighbour position is invalid");
                        continue;
                    }

                    var neighbourIndex = TileMapUtils.CalculateIndex(neighbourPosition.x, neighbourPosition.y, _gridSize.x);
                    var neighbourNode = _pathNodesArray[neighbourIndex];
                    
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.previousIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        _pathNodesArray[neighbourIndex] = neighbourNode;

                        if (!openNodes.Contains(neighbourNode.index))
                        {
                            openNodes.Add(neighbourNode.index);
                        }
                    }
                    
                    Debug.Log("offset processed");
                }
                
                Debug.Log("node position processed");
            }

            var endNode = _pathNodesArray[endNodeIndex];
            if (endNode.index == NODE_INVALID_INDEX)
            {
                Debug.LogError("No path found");
            }
            else
            {
                Debug.Log("Path found:");
                var path = CollectPath(endNode);

                foreach (var point in path)
                {
                    Debug.Log(point);
                }
            }
            
            openNodes.Dispose();
            closedNodes.Dispose();
            offsets.Dispose();
        }

        private static NativeArray<int2> GetOffsets()
        {
            NativeArray<int2> offsets = new NativeArray<int2>(8, Allocator.Temp);
            offsets[0] = new int2(1, 0); // Right
            offsets[1] = new int2(-1, 0); // Left
            offsets[2] = new int2(0, 1); // Up
            offsets[3] = new int2(0, -1); // Down
            offsets[4] = new int2(1, 1); // Right-Up
            offsets[5] = new int2(1, -1); // Right-Down
            offsets[6] = new int2(-1, 1); // Left-Up
            offsets[7] = new int2(-1, -1); // Left-Down
            return offsets;
        }

        private bool NeighbourIsValid(int2 neighbourPosition, NativeList<int> closedNodes)
        {
            int index = TileMapUtils.CalculateIndex(neighbourPosition.x, neighbourPosition.y, _gridSize.x);
            return IsInGrid(neighbourPosition) &&
                   !closedNodes.Contains(index) && 
                   _pathNodesArray[index].walkable;
        }

        private bool IsInGrid(int2 position)
        {
            return position is { x: >= 0, y: >= 0 } && 
                   position.x < _gridSize.x && position.y < _gridSize.y;
        }

        private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);

            int remainingDistance = math.abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remainingDistance;
        }

        private int GetLowestFCostNodeIndex(NativeList<int> openNodes, NativeArray<PathNode> pathNodes)
        {
            PathNode resultNode = _pathNodesArray[openNodes[0]];

            for (int i = 0; i < openNodes.Length; i++)
            {
                PathNode node = pathNodes[i];
                if (node.fCost < resultNode.fCost)
                {
                    resultNode = node;
                }
            }

            return resultNode.index;
        }

        private NativeList<int2> CollectPath(PathNode endNode)
        {
            if (endNode.index == NODE_INVALID_INDEX)
            {
                return new NativeList<int2>(Allocator.Temp);
            }

            var path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y));
            
            var currentNode = endNode;
            while (currentNode.previousIndex != NODE_INVALID_INDEX)
            {
                Debug.Log("addind point");
                currentNode = _pathNodesArray[currentNode.previousIndex];
                path.Add(new int2(currentNode.x, currentNode.y));
            }

            return path;
        }
        
        private struct PathNode
        {
            public int x;
            public int y;

            public int index;
            public int previousIndex;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool walkable;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
        }
    }
}