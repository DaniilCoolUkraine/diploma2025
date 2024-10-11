using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class DestroyColliders : OdinEditorWindow
    {
        [SerializeField] private Transform _parent;
        
        [MenuItem("Tools/DestroyColliders")]
        private static void OpenWindow()
        {
            GetWindow<DestroyColliders>().Show();
        }

        [Button("Destroy All Mesh Colliders")]
        public void DestroyAllMeshColliders()
        {
            DestroyAllCollidersOfType<MeshCollider>(_parent);
        }
        
        [Button("Destroy All Box Colliders")]
        public void DestroyAllBoxColliders()
        {
            DestroyAllCollidersOfType<BoxCollider>(_parent);
        }

        private void DestroyAllCollidersOfType<T>(Transform parent) where T : Collider
        {
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent<T>(out var collider))
                {
                    DestroyImmediate(collider);
                    EditorUtility.SetDirty(child.gameObject);
                }
                
                if (child.childCount != 0)
                {
                    DestroyAllCollidersOfType<T>(child);
                }
            }
        }
    }
}