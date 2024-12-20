using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using Unity.VisualScripting;

namespace FESStateSystem
{
    public class StateCreatorWindow : EditorWindow
    {
        private string stateName = "";
        private string realStateName = "";
        private bool isAbstract;
        private string scriptName;
        private string className;
        private bool attachInherited = false;
        private bool prefixInherited = true;
        private bool encapsulateField = false;
        private MonoScript encapsulates;
        private string sourceUsingLine;
    
        private MonoScript inheritsFromState = null;
        private Type inheritsBaseclass = typeof(AbstractGameplayStateScriptableObject);
        private bool isInheritable;
        
        private string actorTarget;
        
        private static string savePath = "Assets/Scripts/AuthoredStateSystem/GameplayStates";

        [MenuItem("StateSystem/State Creator")]
        public static void ShowWindow()
        {
            GetWindow<StateCreatorWindow>("State Creator");
        }
    
        private void OnGUI()
        {
            GUILayout.Label("Create a New State", EditorStyles.boldLabel);

            stateName = EditorGUILayout.TextField("State Name", stateName);
        
            inheritsFromState = (MonoScript)EditorGUILayout.ObjectField(
                "Inherits From", 
                inheritsFromState, 
                typeof(MonoScript), 
                false // Set to 'true' if you want to allow selecting objects from the scene
            );

            if (isInheritable && inheritsFromState is not null && !string.IsNullOrEmpty(stateName))
            {
                EditorGUI.indentLevel = 1;
                attachInherited = EditorGUILayout.Toggle("Attach Name", attachInherited);
                if (attachInherited)
                {
                    prefixInherited = EditorGUILayout.Toggle("As Prefix", prefixInherited);
                }
                
                string inheritedState = inheritsFromState.GetClass().ToString();
                string[] sourceNameStrings = inheritedState.Split('.');
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
                else
                {
                    sourceUsingLine = "";
                }
                
                inheritedState = inheritedState.Split('.')[^1];
                inheritedState = inheritedState.Replace("GameplayStateScriptableObject", "");
                inheritedState = inheritedState.Replace("Abstract", "");

                actorTarget = inheritedState;
            
                if (attachInherited)
                    realStateName = prefixInherited ? inheritedState + stateName : stateName + inheritedState;
                else realStateName = stateName;
            }
            else realStateName = stateName;
        
            EditorGUI.indentLevel = 0;
        
            if (string.IsNullOrEmpty(realStateName)) scriptName = "";
            else
            {
                scriptName = isAbstract ? $"Abstract{realStateName}GameplayStateScriptableObject" : $"{realStateName}GameplayStateScriptableObject";
                scriptName = scriptName.Replace(" ", "");
            }
        
            if (string.IsNullOrEmpty(realStateName)) className = "";
            else
            {
                className = isAbstract ? $"Abstract{realStateName}GameplayState" : $"{realStateName}GameplayState";
                className = className.Replace(" ", "");
            }
        
            isAbstract = EditorGUILayout.Toggle("Make Abstract", isAbstract);
            if (isAbstract)
            {
                EditorGUI.indentLevel = 1;
                encapsulates = (MonoScript)EditorGUILayout.ObjectField(
                    "Encapsulates", 
                    encapsulates, 
                    typeof(MonoScript), 
                    false // Set to 'true' if you want to allow selecting objects from the scene
                );
                EditorGUI.indentLevel = 0;
            }
            else
            {
                encapsulates = null;
            }
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", scriptName);
            EditorGUILayout.TextField("", className);
            if (!string.IsNullOrEmpty(sourceUsingLine))
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.TextField("Dependency", sourceUsingLine.Replace("using ", "").Replace(";", ""));
                EditorGUI.indentLevel = 0;
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space(10);

            try
            {
                if (inheritsFromState is not null)
                {
                    Type inheritType = inheritsFromState.GetClass();
                    if (inheritType is not null)
                    {
                        isInheritable = inheritsBaseclass.IsAssignableFrom(inheritType) && inheritType.IsAbstract;
                        if (!isInheritable)
                        {
                            inheritsFromState = null;
                        }
                    }
                }
            }
            catch (UnassignedReferenceException)
            {
                isInheritable = false;
            }
            
            EditorGUILayout.Space(10);
            
            GUILayout.Label("Asset Menu", EditorStyles.boldLabel);
            
            actorTarget = EditorGUILayout.TextField("Actor Target", actorTarget);

            string menuTarget = !string.IsNullOrEmpty(actorTarget) ? $"{actorTarget}/{stateName} State" : $"General/{stateName} State";
            string menuPath = !isAbstract
                ? $"FESState/Authored/State/{menuTarget}"
                : "No menu path for abstract classes";
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", string.IsNullOrEmpty(stateName) ? "" : menuPath);
            EditorGUI.EndDisabledGroup();
        
            EditorGUILayout.Space(10);
            
            GUILayout.Label("Save Path", EditorStyles.boldLabel);
            savePath = EditorGUILayout.TextField("Path", savePath);
        
            EditorGUILayout.Space(15);
            

            if (File.Exists(Path.Combine(savePath + '/', scriptName + ".cs")))
            {
                GUILayout.Label("Cannot create state: This file already exists at this path.");
            }
            else if (string.IsNullOrEmpty(stateName))
            {
                GUILayout.Label("Cannot create state: State must have a valid name.");
            }
            else if (GUILayout.Button("Create State"))
            {
                CreateStateScript();
                ResetStateStuff();
            }
        }

        private void ResetStateStuff()
        {
            // inheritsFromState = null;
            // isInheritable = false;

            attachInherited = false;
            prefixInherited = true;
    
            stateName = "";
            realStateName = "";
            isAbstract = false;
            scriptName = "";
            className = "";
            sourceUsingLine = "";

            // actorTarget = "";
        }

        private void CreateStateScript()
        {
            string inheritedFrom = isInheritable && inheritsFromState is not null
                ? inheritsFromState.GetClass().ToString()
                : "AbstractGameplayStateScriptableObject";
            if (!string.IsNullOrEmpty(sourceUsingLine)) inheritedFrom = inheritedFrom.Replace(sourceUsingLine.Replace("using ", "").Replace(";", ""), "").Replace(".", "");
            string subInheritedFrom = inheritsFromState is not null
                ? inheritedFrom.Replace("ScriptableObject", "") 
                : "AbstractGameplayState";
            if (!string.IsNullOrEmpty(sourceUsingLine)) subInheritedFrom = subInheritedFrom.Replace(sourceUsingLine.Replace("using ", "").Replace(";", ""), "").Replace(".", "");
            string menuTarget = !string.IsNullOrEmpty(actorTarget) ? $"{actorTarget}/State/{stateName} State" : $"General/{stateName} State";
            string header = isAbstract ? "" : $"[CreateAssetMenu(menuName = \"FESState/Authored/{menuTarget}\", fileName = \"{realStateName}State\")]";
            string abstractTag = isAbstract ? "abstract " : "";

            string encapsulatesName = "";
            string classMembers = "";
            string classInit = "";
            if (isAbstract && encapsulates is not null)
            {
                encapsulatesName = encapsulates.GetClass().ToString();
                encapsulatesName = encapsulatesName.Replace("ScriptableObject", "");
                encapsulatesName = encapsulatesName.Replace("Controller", "");
                encapsulatesName = encapsulatesName.Replace("Manager", "");

                classMembers = $"public {encapsulates.GetClass()} {encapsulatesName};";
                classInit = $"{encapsulatesName} = State.GetComponent<{encapsulates.GetClass()}>();";
            }

            string scriptableObjectInternals = !isAbstract
                ? $@"
    public override List<AbstractGameplayState> GenerateStates(StateActor actor)
    {{
        return new List<AbstractGameplayState>()
        {{
            new {className}(this, actor)
        }};
    }}
"
                : "";
            string subclassInternals = !isAbstract 
                ? $@"
        public override void Initialize()
        {{

        }}

        public override void Enter()
        {{
            
        }}
        public override void LogicUpdate()
        {{
            
        }}
        public override void PhysicsUpdate()
        {{
            
        }}
        public override void Interrupt()
        {{
            
        }}
        public override void Conclude()
        {{
            // Base implementation transitions to relevant initial moderator state
            base.Conclude();
        }}
        public override void Exit()
        {{
            
        }}
" 
                : $@"
        public override void Initialize()
        {{
            {classInit}
        }}
";
            string usingString = "using UnityEngine;\nusing System.Collections.Generic;\nusing FESStateSystem;";
            if (!string.IsNullOrEmpty(sourceUsingLine))
            {
                usingString += $"\n{sourceUsingLine}";
            }

            string scriptTemplate = $@"{usingString}

{header}
public {abstractTag}class {scriptName} : {inheritedFrom}
{{
    {scriptableObjectInternals}
    public {abstractTag}class {className} : {subInheritedFrom}
    {{
        {classMembers}
        public {className}(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {{
            
        }}
        {subclassInternals}
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