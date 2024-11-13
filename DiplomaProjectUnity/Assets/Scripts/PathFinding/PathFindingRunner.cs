using System;
using System.Collections.Generic;
using DiplomaProject.General;
using DiplomaProject.PathFinding.Finders;
using DiplomaProject.PathFinding.Utils;
using DiplomaProject.TileMap;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.PathFinding
{
    public class PathFindingRunner : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
        [SerializeField] private Vector2Int _startPoint = new Vector2Int(0, 0);
        [SerializeField] private Vector2Int _endPoint = new Vector2Int(9, 9);

        [SerializeField] private AStar _aStar;

        [SerializeField, HideInInspector] private List<TileNode> _nodes = new();
        
        private void Start()
        {
            _aStar.FindPath(new int2(_startPoint.x, _startPoint.y), new int2(_endPoint.x, _endPoint.y), _nodes, new int2(_gridSize.x, _gridSize.y));
        }

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