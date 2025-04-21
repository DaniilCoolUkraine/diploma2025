using DiplomaProject.Goap;
using UnityEditor;

namespace DiplomaProject.ToolsWindows.Editor
{
    [CustomEditor(typeof(GoapAgent))]
    public class GoapAgentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var agent = (GoapAgent)target;
            
            // PrefixLabel($"Goal: {agent.CurrentGoal?.Name ?? "None"}");
        }
    }
}