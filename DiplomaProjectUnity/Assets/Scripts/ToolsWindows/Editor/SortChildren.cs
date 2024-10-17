using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DiplomaProject.ToolsWindows.Editor
{
    public class SortChildren : OdinEditorWindow
    {
        [SerializeField] private Transform _parent;

        [MenuItem("Tools/Sort Children")]
        private static void OpenWindow()
        {
            GetWindow<SortChildren>().Show();
        }

        [Button("Sort Children")]
        public void Sort()
        {
            // Create a list to store the child objects
            List<Transform> children = new List<Transform>();

            // Populate the list with the children of the parent
            foreach (Transform child in _parent)
            {
                children.Add(child);
            }

            // Sort the list based on their names, extracting the numeric value and using it for sorting
            children = children.OrderBy(child =>
            {
                // Match the prefix and extract the numeric part at the end of the name
                string name = child.name;
                string prefix = name.Substring(0, name.LastIndexOf('_'));
                int number = int.Parse(name.Substring(name.LastIndexOf('_') + 1));
                return (prefix, number);
            }).ToList();

            // Reorder the children in the hierarchy
            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
            
            EditorUtility.SetDirty(_parent.gameObject);
        }

    }
}