using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiplomaProject.General;
using DiplomaProject.General.Extensions;
using DiplomaProject.PathFinding;
using DiplomaProject.PathFinding.Finders;
using DiplomaProject.PathFinding.Utils;
using DiplomaProject.TileMap;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class TileMapDrawer : OdinEditorWindow
    {
        [SerializeField] private PathFindingRunner _runner;

        [SerializeField] private Vector2Int _gridSize;

        private List<TileNode> _nodes;

        private readonly Vector2 _tileSize = new(8, 8);

        [MenuItem("Tools/Draw map")]
        private static void OpenWindow()
        {
            GetWindow<TileMapDrawer>().Show();
        }

        private void OnValidate()
        {
            RefreshMap();
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();
            
            if (_runner == null)
            {
                return;
            }
            
            GUILayout.BeginVertical();
            for (int y = _gridSize.y - 1; y >= 0; y--)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < _gridSize.x; x++)
                {
                    TileNode node = _nodes[y * _gridSize.x + x];
                    
                    GUI.backgroundColor = node.IsWalkable ? Color.white : Color.gray;
        
                    if (GUILayout.Button($"", GUILayout.Width(_tileSize.x), GUILayout.Height(_tileSize.y)))
                    {
                        OnTileClicked(node);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            
            GUI.backgroundColor = Color.white;
        }

        private void RefreshMap()
        {
            if (_runner == null)
            {
                return;
            }
            
            _nodes = _runner.GetMap().ToList().ConvertAll(node => new TileNode(node));
            if (_nodes.Count < _gridSize.y * _gridSize.x)
            {
                _nodes = new List<TileNode>();
                for (int y = 0; y < _gridSize.y; y++)
                {
                    for (int x = 0; x < _gridSize.x; x++)
                    {
                        _nodes.Add(new TileNode(x, y));
                    }
                }
            }
        }

        private void OnTileClicked(TileNode node)
        {
            node.IsWalkable = !node.IsWalkable;
        }

        [Button]
        private void SaveMap()
        {
            _runner.SetMap(_gridSize, _nodes);
            EditorUtility.SetDirty(_runner);

            ConvertMapToJson(_gridSize, _nodes);
        }

        private void ConvertMapToJson(Vector2Int gridSize, List<TileNode> nodes)
        {
            var tilemap = SetupGrid(gridSize.ToInt2(), nodes);
            
            var sizeString = JsonConvert.SerializeObject(gridSize);
            var tilemapString = JsonConvert.SerializeObject(tilemap);
            
            var tilemapPath = "Assets/Config/Tilemaps/TestTilemap.json";
            var sizePath = "Assets/Config/Tilemaps/TestTilemapSize.json";
            
            File.WriteAllText(tilemapPath, tilemapString);
            File.WriteAllText(sizePath, sizeString);

            tilemap.Dispose();
        }

        private NativeArray<PathNode> SetupGrid(int2 gridSize, List<TileNode> nodes)
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
                    pathNode.hCost = TileMapUtils.CalculateDistanceCost(new int2(x, y), new int2(0, 0));
                    pathNode.CalculateFCost();
    
                    pathNode.Walkable = nodes[pathNode.Index].IsWalkable;
    
                    pathNodeArray[pathNode.Index] = pathNode;
                }
            }
    
            return pathNodeArray;
        }
        
        [Button]
        private void ClearMap()
        {
            _gridSize = Vector2Int.zero;
            _nodes.Clear();

            SaveMap();
        }
    }
}