using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public class TransitionBehaviourComponentCreatorWindow : EditorWindow
    {
        private string scriptName;
        private MonoScript sourceType;
        private string sourceName;
        private string sourceUsingLine;

        private static string savePath = "Assets/Scripts/AuthoredStateSystem/TransitionBehaviourComponents";

        [MenuItem("StateSystem/Transition Behaviour/Component Creator")]
        public static void ShowWindow()
        {
            GetWindow<TransitionBehaviourComponentCreatorWindow>("Transition Behaviour Component Creator");
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
                
                string[] sourceNameStrings = sourceName.Split('.');
                if (sourceNameStrings.Length > 1)
                {
                    sourceUsingLine = "using ";
                    for (int i = 0; i < sourceNameStrings.Length - 1; i++)
                    {
                        sourceUsingLine += sourceNameStrings[i];
                        if (i < sourceNameStrings.Length - 2) sourceUsingLine += ".";
                    }

                    sourceUsingLine += ";";
                }
                
                sourceName = sourceNameStrings[^1];
                scriptName = $"{sourceName}TransitionBehaviourComponent";
            }
            else
            {
                sourceName = "";
                scriptName = "";
                sourceUsingLine = "";
            }
            
            EditorGUI.indentLevel = 0;
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", scriptName);
            EditorGUILayout.TextField("", sourceName);
            if (!string.IsNullOrEmpty(sourceUsingLine))
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.TextField("Dependency", $"{sourceUsingLine.Replace(";", "").Replace("using ", "")}");
                EditorGUI.indentLevel = 0;
            }
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
            string usingLines = "using UnityEngine;\nusing FESStateSystem;";
            if (!string.IsNullOrEmpty(sourceUsingLine))
            {
                usingLines += $"\n{sourceUsingLine}";
            }
            string scriptTemplate = $@"{usingLines}

/// <summary>
/// A transition behaviour component that interfaces with {sourceName}.
/// </summary>
[RequireComponent(typeof({sourceName}))]
public class {scriptName} : AbstractTransitionBehaviourComponent<{sourceName}>
{{
    /*
    Override options:
        OnAwakeEvent
        OnStartEvent
        
    Methods of interest:
        Run(StateTransition<{sourceName}> transition,
            StateContextTagScriptableObject contextTag,
            bool interruptStateChange = true
        )
        RunAll(bool interruptStateChange = true)
        RunWithinContext(StateContextTagScriptableObject contextTag,
            bool interruptStateChange = true
        )
    */

    // Implement behaviour as needed
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
