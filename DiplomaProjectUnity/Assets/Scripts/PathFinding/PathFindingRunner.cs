using System;
using System.Collections.Generic;
using DiplomaProject.PathFinding.Utils;
using DiplomaProject.TileMap;
using Shapes;
using UnityEngine;

namespace DiplomaProject.PathFinding
{
    public class PathFindingRunner : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);

        [SerializeField, HideInInspector] private List<TileNode> _nodes = new();

        private void OnDrawGizmos()
        {
            TileMapUtils.DrawGrid(_nodes, transform);
        }

        public IEnumerable<TileNode> GetMap()
        {
            return _nodes;
        }
        
        public void SetMap(Vector2Int gridSize, List<TileNode> nodes)
        {
            _nodes = nodes;
            _gridSize = gridSize;
        }
    }
}