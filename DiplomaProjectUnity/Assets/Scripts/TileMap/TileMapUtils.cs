using System.Collections.Generic;
using DiplomaProject.General;
using DiplomaProject.PathFinding.Utils;
using Shapes;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.TileMap
{
    public static class TileMapUtils
    {
        public static int CalculateIndex(int x, int y, int gridWidth)
        {
            // Debug.Log($"calculate index for ({x}, {y}), gridWidth {gridWidth}. index is {x + y * gridWidth}");
            return x + y * gridWidth;
        }

        public static void DrawGrid(IEnumerable<TileNode> nodes, Transform parent)
        {
            var size = new Vector2(Constants.TILE_PER_UNIT, Constants.TILE_PER_UNIT);

            foreach (var node in nodes)
            {
                var textElement = new TextElement();

                var color = node.IsWalkable ? new Color(0,0,0,0.5f) : new Color(1,1,1,0.5f);
                var worldPosition = TileToWorldPosition(node.X, node.Y);
                var cellCenter = parent.position + new Vector3(worldPosition.x, 0, worldPosition.y);

                Draw.Rectangle(cellCenter, Quaternion.Euler(90, 0, 0), size, Constants.TILE_BORDER_RADIUS, color);
                // Draw.Text(textElement, cellCenter, $"{node.X}, {node.Y}", 3, Color.green);

                textElement.Dispose();   
            }
        }
        
        public static Vector3 TileToWorldPosition(int x, int y)
        {
            return new Vector3(x * Constants.TILE_PER_UNIT, 0, y * Constants.TILE_PER_UNIT);
        }

        public static bool TilePositionIsInBounds(int x, int y, int gridWidth, int gridHeight)
        {
            return x >= 0 && y >= 0 && 
                   x < gridWidth && y < gridHeight;
        }
        
        public static int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            
            int remaining = math.abs(xDistance - yDistance);
            return PathFinderConstants.MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) +
                   PathFinderConstants.MOVE_STRAIGHT_COST * remaining;
        }
    }
}