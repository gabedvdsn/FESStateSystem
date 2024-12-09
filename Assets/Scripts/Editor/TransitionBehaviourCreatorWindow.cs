using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public class TransitionBehaviourCreatorWindow : EditorWindow
    {
        private string scriptName;
        private MonoScript sourceType;
        private string sourceName;

        private string savePath = "Assets/Scripts/AuthoredStateSystem/TransitionBehaviours";

        [MenuItem("StateSystem/Transition Behaviour Creator")]
        public static void ShowWindow()
        {
            GetWindow<TransitionBehaviourCreatorWindow>("Transition Behaviour Creator");
        }
    
        private void OnGUI()
        {
            GUILayout.Label("Create a New Transition Behaviour", EditorStyles.boldLabel);

            sourceType = (MonoScript)EditorGUILayout.ObjectField(
                "Source Type", 
                sourceType, 
                typeof(MonoScript), 
                false // Set to 'true' if you want to allow selecting objects from the scene
            );

            if (sourceType != null)
            {
                sourceName = sourceType.GetClass().ToString();
                sourceName = sourceName.Split('.')[^1];
                scriptName = $"{sourceName}StateTransitionBehaviour";
            }
            else
            {
                sourceName = "";
                scriptName = "";
            }
            
            EditorGUI.indentLevel = 0;
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", scriptName);
            EditorGUILayout.TextField("", sourceName);
            EditorGUI.EndDisabledGroup();
        
            EditorGUILayout.Space(10);
            GUILayout.Label("Save Path", EditorStyles.boldLabel);

            savePath = EditorGUILayout.TextField("Path", savePath);
        
            EditorGUILayout.Space(15);

            if (File.Exists(Path.Combine(savePath + '/', scriptName + ".cs")))
            {
                GUILayout.Label("Cannot create transition behaviour: This file already exists at this path.");
            }
            else if (sourceType is null || string.IsNullOrEmpty(scriptName))
            {
                GUILayout.Label("Cannot create transition behaviour: Must have a valid source type.");
            }
            else if (GUILayout.Button("Create Transition Behaviour"))
            {
                CreateTransitionBehaviourScript();
                ResetStateStuff();
            }
        }

        private void ResetStateStuff()
        {
            scriptName = "";
            sourceType = null;
            sourceName = "";
        }

        private void CreateTransitionBehaviourScript()
        {
            
            string scriptTemplate = $@"using UnityEngine;
using FESStateSystem;

[RequireComponent(typeof({sourceName}))]
public class {scriptName} : AbstractStateTransitionComponent<{sourceName}>
{{

}}

";

            string folderPath = savePath + "/";
            string filePath = Path.Combine(folderPath, scriptName + ".cs");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Write the script to the file
            File.WriteAllText(filePath, scriptTemplate);

            // Refresh the Asset Database to show the new script
            AssetDatabase.Refresh();
        }
    }
}
