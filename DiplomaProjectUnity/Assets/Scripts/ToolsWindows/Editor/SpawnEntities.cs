using System.IO;
using DiplomaProject.PathFinding.Finders;
using DiplomaProject.PathFinding.Followers;
using DiplomaProject.TileMap;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class SpawnEntities : OdinEditorWindow
    {
        [SerializeField] private Transform _rootEntity;
        [SerializeField, PreviewField] private Transform[] _modelsVariation;

        [SerializeField] private int _entitiesCount;
        
        [MenuItem("Tools/Spawn Entities")]
        private static void OpenWindow()
        {
            GetWindow<SpawnEntities>().Show();
        }
        
        [Button("Spawn Entities")]
        public void SpawnEntitiesOnScene()
        {
            var tilemapPath = "Assets/Config/Tilemaps/TestTilemap.json";
            var tilemapJson = File.ReadAllText(tilemapPath);
            
            PathNode[] array = JsonConvert.DeserializeObject<PathNode[]>(tilemapJson);
            
            for (int i = 0; i < _entitiesCount; i++)
            {
                var modelReference = _modelsVariation[Random.Range(0, _modelsVariation.Length)];

                var entity = Instantiate(_rootEntity);
                var model = Instantiate(modelReference, entity);
                
                entity.name = entity.name.Replace("(Clone)", $"{i}");
                
                var entityTransform = entity.GetComponent<Transform>();
                var entityCurrentPosition = entity.GetComponent<CurrentPositionAuthoring>();
                
                while (true)
                {
                    var tile = array[Random.Range(0, array.Length)];
                    if (tile.Walkable)
                    {
                        entityCurrentPosition.SetCurrentPosition(new Vector2Int(tile.X, tile.Y));
                        entityTransform.position = TileMapUtils.TileToWorldPosition(tile.X, tile.Y);
                        break;
                    }
                }
                
                EditorUtility.SetDirty(entity);
                EditorUtility.SetDirty(model);
            }
        }
    }
}