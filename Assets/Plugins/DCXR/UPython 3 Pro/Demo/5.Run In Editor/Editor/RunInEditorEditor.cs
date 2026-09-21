using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    [CustomEditor(typeof(RunInEditor))]
    public class RunInEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            RunInEditor tar = target as RunInEditor;

            if (GUILayout.Button("Submit"))
            {
                tar.Call();
            }
            
            if (GUILayout.Button("Clear Result"))
            {
                tar.resultText.text = "";
            }
            
            if (GUILayout.Button("UPython Stop"))
            {
                UPython.Instance.Stop();
            }
            
            if (GUILayout.Button("UPython Restart"))
            {
                UPython.Instance.ReStart();
            }
            
        }
    }
}