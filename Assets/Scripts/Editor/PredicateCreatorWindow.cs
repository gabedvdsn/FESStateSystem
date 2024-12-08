using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    /// <summary>
    /// Encapsulate some data S
    /// </summary>
    public class PredicateCreatorWindow : EditorWindow
    {
        private string predicateName;
        private string scriptName;
        private string subclassName;
        private string assetMenuString;
        private MonoScript nameSource;

        private bool attachName;
        private bool prefixSource = true;
        
        private string savePath = "Assets/Scripts/StateSystem/Predicates";

        [MenuItem("StateSystem/Predicate Creator")]
        public static void ShowWindow()
        {
            GetWindow<PredicateCreatorWindow>("Predicate Creator");
        }
    
        private void OnGUI()
        {
            GUILayout.Label("Create a New Predicate", EditorStyles.boldLabel);

            predicateName = EditorGUILayout.TextField("State Name", predicateName);
        
            nameSource = (MonoScript)EditorGUILayout.ObjectField(
                "Relative Source", 
                nameSource, 
                typeof(MonoScript), 
                false // Set to 'true' if you want to allow selecting objects from the scene
            );

            if (!string.IsNullOrEmpty(predicateName))
            {
                scriptName = $"{predicateName}TransitionPredicateScriptableObject";
                subclassName = $"{predicateName}TransitionPredicate";
            }
            
            if (nameSource is not null && !string.IsNullOrEmpty(predicateName))
            {
                EditorGUI.indentLevel = 1;
                attachName = EditorGUILayout.Toggle("Attach Name", attachName);
                if (attachName)
                {
                    prefixSource = EditorGUILayout.Toggle("As Prefix", prefixSource);
                }

                if (attachName)
                {
                    string sourceName = nameSource.GetClass().ToString();
                    sourceName = sourceName.Replace("Controller", "");
                    sourceName = sourceName.Replace("System", "");
                    sourceName = sourceName.Replace("Manager", "");
                    sourceName = sourceName.Replace("Abstract", "");

                    if (prefixSource)
                    {
                        scriptName = $"{sourceName}{predicateName}TransitionPredicateScriptableObject";
                        subclassName = $"{sourceName}{predicateName}TransitionPredicate";
                    }
                    else
                    {
                        scriptName = $"{predicateName}{sourceName}TransitionPredicateScriptableObject";
                        subclassName = $"{predicateName}{sourceName}TransitionPredicate";
                    }
                }
            }

            assetMenuString = $"FESState/Authored/State/Transition Predicate/{predicateName}";
            
            EditorGUI.indentLevel = 0;
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", scriptName);
            EditorGUILayout.TextField("", subclassName);
            EditorGUI.EndDisabledGroup();
        
            EditorGUILayout.Space(10);
        
            savePath = EditorGUILayout.TextField("Path", savePath);
        
            EditorGUILayout.Space(15);

            if (File.Exists(Path.Combine(savePath + '/', scriptName + ".cs")))
            {
                GUILayout.Label("Cannot create predicate: This file already exists at this path.");
            }
            else if (string.IsNullOrEmpty(predicateName))
            {
                GUILayout.Label("Cannot create predicate: Must have a valid name.");
            }
            else if (GUILayout.Button("Create Predicate"))
            {
                CreateStateScript();
                ResetStateStuff();
            }
        }

        private void ResetStateStuff()
        {
            attachName = false;
            prefixSource = true;
            nameSource = null;
            
            scriptName = "";
            subclassName = "";
        }

        private void CreateStateScript()
        {
            
            string scriptTemplate = $@"using UnityEngine;
using FESStateSystem;

[CreateAssetMenu(menuName = ""{assetMenuString}"")]
public class {scriptName} : AbstractTransitionPredicateScriptableObject
{{
    public override AbstractTransitionPredicate<S> GeneratePredicate<S>(S source)
    {{
        return new {subclassName}<S>(source);
    }}

    public class {subclassName}<S> : AbstractTransitionPredicate<S>
    {{
        public {subclassName}(S source) : base(source)
        {{
        }}
        
        public override bool Evaluate()
        {{
            return true;
        }}
    }}
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
