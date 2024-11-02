using System.Collections.Generic;
using DiplomaProject.PathFinding.Utils;
using DiplomaProject.TileMap;
using UnityEngine;

namespace DiplomaProject.PathFinding
{
    public class PathFindingRunner : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(10, 10);
        [SerializeField] private Vector2Int _startPoint = new Vector2Int(0, 0);
        [SerializeField] private Vector2Int _endPoint = new Vector2Int(9, 9);

        [SerializeField, HideInInspector] private List<TileNode> _nodes = new();
        
        private void Start()
        {
            var algorithm = new Astar();

            algorithm.CreateGrid(_gridSize);
            algorithm.FindPath(_startPoint, _endPoint);
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