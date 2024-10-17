using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class MeshCombiner : OdinEditorWindow
    {
        [SerializeField] private MeshFilter[] _meshes;
        [SerializeField] private MeshFilter _targetMesh;

        [MenuItem("Tools/Combine Meshes")]
        private static void OpenWindow()
        {
            GetWindow<MeshCombiner>().Show();
        }
        
        [Button("Combine Meshes")]
        public void CombineMeshes()
        {
            var combine = new CombineInstance[_meshes.Length];

            int i = 0;
            foreach (var mesh in _meshes)
            {
                combine[i].mesh = mesh.sharedMesh;
                combine[i].transform = mesh.transform.localToWorldMatrix;

                i++;
            }
            
            var newMesh = new Mesh();
            newMesh.CombineMeshes(combine);
            
            _targetMesh.mesh = newMesh;
            _targetMesh.GetComponent<MeshRenderer>().sharedMaterials = _meshes[0].GetComponent<MeshRenderer>().sharedMaterials;

            EditorUtility.SetDirty(_targetMesh);

            foreach (var mesh in _meshes)   
            {
                mesh.gameObject.SetActive(false);
                EditorUtility.SetDirty(mesh);
            }
            
            SaveMesh(newMesh, true);
        }

        private void SaveMesh(Mesh mesh, bool optimize)
        {
            var path = EditorUtility.SaveFilePanel("Save combined mesh", "Assets", _targetMesh.name, "asset");
            if (string.IsNullOrWhiteSpace(path)) return;
            path = FileUtil.GetProjectRelativePath(path);

            if (optimize)
            {
                MeshUtility.Optimize(mesh);
            }
            
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }
    }
}