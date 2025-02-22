﻿using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class MeshCombiner : OdinEditorWindow
    {
        [Header("Optionals")] 
        [SerializeField] private bool _addMeshCollider;
        
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

            var targetTransform = _targetMesh.transform.worldToLocalMatrix;
            
            int i = 0;
            foreach (var mesh in _meshes)
            {
                combine[i].mesh = mesh.sharedMesh;
                combine[i].transform = targetTransform * mesh.transform.localToWorldMatrix;

                i++;
            }
            
            var newMesh = new Mesh();
            newMesh.CombineMeshes(combine);
            
            _targetMesh.mesh = newMesh;
            _targetMesh.GetComponent<MeshRenderer>().sharedMaterials = _meshes[0].GetComponent<MeshRenderer>().sharedMaterials;

            if (_addMeshCollider)
            {
                var collider = _targetMesh.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = newMesh;
            }

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