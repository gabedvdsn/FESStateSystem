using System;
using UnityEditor;
using UnityEngine;
using System.IO;

public class StateCreatorWindow : EditorWindow
{
    private string stateName = "";
    private string realStateName = "";
    private bool isAbstract;
    private string scriptName;
    private string className;
    private bool attachInherited = true;
    private bool prefixInherited = true;
    
    private MonoScript inheritsFromState;
    private Type inheritsBaseclass = typeof(AbstractGameplayStateScriptableObject);
    private bool isInheritable;
    
    private string actorTarget;

    private string savePath = "Assets/Scripts/GameplayStates";

    [MenuItem("Tools/State Creator")]
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
            inheritedState = inheritedState.Replace("GameplayStateScriptableObject", "");
            inheritedState = inheritedState.Replace("Abstract", "");

            if (string.IsNullOrEmpty(actorTarget)) actorTarget = inheritedState;
            
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
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("", scriptName);
        EditorGUILayout.TextField("", className);
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
        
        actorTarget = EditorGUILayout.TextField("Actor Target", actorTarget);
        string menuTarget = !string.IsNullOrEmpty(realStateName) ? $"{realStateName} State" : "";
        string menuPath = !isAbstract
            ? $"FESStates/Actor/{(!string.IsNullOrEmpty(actorTarget) ? $"{actorTarget}/{menuTarget}" : $"General/{menuTarget}")}"
            : "No menu path for abstract classes";
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("", menuPath);
        EditorGUI.EndDisabledGroup();
        
        savePath = EditorGUILayout.TextField("Path", savePath);
        
        EditorGUILayout.Space(15);

        if (File.Exists(Path.Combine(savePath + '/', scriptName + ".cs")))
        {
            GUILayout.Label("Cannot create state: This file already exists at this path.");
        }
        else if (GUILayout.Button("Create State"))
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogError("State name cannot be empty!");
                return;
            }

            CreateStateScript();
            ResetStateStuff();
        }
    }

    private void ResetStateStuff()
    {
        inheritsFromState = null;
        isInheritable = false;
    
        stateName = "";
        realStateName = "";
        isAbstract = false;
        scriptName = "";
        className = "";

        actorTarget = "";
    }

    private void CreateStateScript()
    {
        string inheritedFrom = inheritsFromState is not null
            ? inheritsFromState.GetClass().ToString()
            : "AbstractGameplayStateScriptableObject";
        string subInheritedFrom = inheritsFromState is not null
            ? inheritedFrom.Replace("ScriptableObject", "") : "AbstractGameplayState";
        string menuTarget = !string.IsNullOrEmpty(actorTarget) ? $"{actorTarget}/{stateName} State" : $"General/{stateName} State";
        string header = isAbstract ? "" : $"[CreateAssetMenu(menuName = \"FESState/Actor/{menuTarget}\")]";
        string abstractTag = isAbstract ? "abstract " : "";

        string scriptableObjectInternals = !isAbstract
            ? $@"
    public override AbstractGameplayState GenerateState(StateActor actor)
    {{
        return new {className}(this, actor);
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
            
        }}
        public override void Exit()
        {{
            
        }}
" 
            : "";
        
        string scriptTemplate = $@"using UnityEngine;

{header}
public {abstractTag}class {scriptName} : {inheritedFrom}
{{
    {scriptableObjectInternals}
    public {abstractTag}class {className} : {subInheritedFrom}
    {{
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