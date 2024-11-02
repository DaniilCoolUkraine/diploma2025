using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public static void DrawGrid(Vector2Int size, Transform parent)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var cellCenter = parent.position + TileToWorldPosition(x, y);
                    var rect = new Rect(cellCenter, new Vector2(Constants.TILE_PER_UNIT, Constants.TILE_PER_UNIT));

                    Draw.Rectangle(cellCenter, Quaternion.identity,rect, Constants.TILE_BORDER_RADIUS, Color.green);
                }
            }
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