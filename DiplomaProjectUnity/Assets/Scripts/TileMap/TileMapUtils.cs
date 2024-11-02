using System.Collections.Generic;
using DiplomaProject.General;
using DiplomaProject.PathFinding.Utils;
using Shapes;
using UnityEngine;

namespace DiplomaProject.TileMap
{
    public static class TileMapUtils
    {
        public static int CalculateIndex(int x, int y, int gridWidth)
        {
            Debug.Log($"calculate index for ({x}, {y}), gridWidth {gridWidth}. index is {x + y * gridWidth}");
            return x + y * gridWidth;
        }

        public static void DrawGrid(IEnumerable<TileNode> nodes, Transform parent)
        {
            var size = new Vector2(Constants.TILE_PER_UNIT, Constants.TILE_PER_UNIT);

            foreach (var node in nodes)
            {
                var textElement = new TextElement();

                var color = node.IsWalkable ? Color.white : Color.black;
                var cellCenter = parent.position + TileToWorldPosition(node.X, node.Y);

                Draw.Rectangle(cellCenter, Quaternion.identity, size, Constants.TILE_BORDER_RADIUS, color);
                Draw.Text(textElement, cellCenter, $"{node.X}, {node.Y}", 3, Color.green);

                textElement.Dispose();   
            }
        }
        
        public static Vector3 TileToWorldPosition(int x, int y)
        {
            return new Vector3(x * Constants.TILE_PER_UNIT, y * Constants.TILE_PER_UNIT, 0);
        }
    }
}