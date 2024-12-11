using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public class PredicateCreatorWindow : EditorWindow
    {
        private string predicateName;
        private string scriptName;
        private string subclassName;
        private string assetMenuString;

        private static string savePath = "Assets/Scripts/AuthoredStateSystem/Predicates";

        [MenuItem("StateSystem/Predicate Creator")]
        public static void ShowWindow()
        {
            GetWindow<PredicateCreatorWindow>("Predicate Creator");
        }
    
        private void OnGUI()
        {
            GUILayout.Label("Create a New Predicate", EditorStyles.boldLabel);

            predicateName = EditorGUILayout.TextField("Predicate Name", predicateName);

            if (!string.IsNullOrEmpty(predicateName))
            {
                scriptName = $"{predicateName}TransitionPredicateScriptableObject";
                subclassName = $"{predicateName}TransitionPredicate";
            }

            assetMenuString = $"FESState/Authored/State/Transition Predicate/{predicateName}";
            
            EditorGUI.indentLevel = 0;
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", scriptName);
            EditorGUILayout.TextField("", subclassName);
            EditorGUI.EndDisabledGroup();
        
            EditorGUILayout.Space(10);
            
            GUILayout.Label("Save Path", EditorStyles.boldLabel);

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
                CreatePredicateScript();
                ResetStateStuff();
            }
        }

        private void ResetStateStuff()
        {
            predicateName = "";
            scriptName = "";
            subclassName = "";
        }

        private void CreatePredicateScript()
        {
            
            string scriptTemplate = $@"using UnityEngine;
using FESStateSystem;
using System.Collections.Generic;

[CreateAssetMenu(menuName = ""{assetMenuString}"")]
public class {scriptName} : AbstractTransitionPredicateScriptableObject
{{
    public override List<AbstractTransitionPredicate<S>> Generate<S>()
    {{
        return new List<AbstractTransitionPredicate<S>>()
        {{
            new {subclassName}<S>()
        }};
    }}

    public class {subclassName}<S> : AbstractTransitionPredicate<S>
    {{
        public override bool Evaluate(S source)
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

        public static void CreatePredicate(string _predicateName, string _subclassName, string _scriptName, string _savePath = "Assets/Scripts/AuthoredStateSystem/Predicates")
        {
            string _assetMenuName = $"FESState/Authored/State/Predicate/{_predicateName}";
            string scriptTemplate = $@"using UnityEngine;
using FESStateSystem;

[CreateAssetMenu(menuName = ""{_assetMenuName}"")]
public class {_scriptName} : AbstractTransitionPredicateScriptableObject
{{
    public override AbstractTransitionPredicate<S> Generate<S>(S source)
    {{
        return new {_subclassName}<S>(source);
    }}

    public class {_subclassName}<S> : AbstractTransitionPredicate<S>
    {{
        public {_subclassName}(S source) : base(source)
        {{
        }}
        
        public override bool Evaluate()
        {{
            return true;
        }}
    }}
}}

";

            string folderPath = _savePath + "/";
            string filePath = Path.Combine(folderPath, _scriptName + ".cs");

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
