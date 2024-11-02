using System.Collections.Generic;
using System.Linq;
using DiplomaProject.PathFinding;
using DiplomaProject.PathFinding.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class TileMapDrawer : OdinEditorWindow
    {
        [SerializeField] private PathFindingRunner _runner;

        [SerializeField] private Vector2Int _gridSize;

        private List<TileNode> _nodes;

        private readonly Vector2 _tileSize = new(40, 40);

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
        
                    if (GUILayout.Button($"{x}, {y}", GUILayout.Width(_tileSize.x), GUILayout.Height(_tileSize.y)))
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