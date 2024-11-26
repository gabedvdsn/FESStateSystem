using System.IO;
using UnityEditor;
using UnityEngine;

public class RetrievalCreatorWindow : EditorWindow
{
    private string retrievalName = "";
    private string scriptName = "";
    private bool useExternalSource;
    private bool useExternalTarget;
    private bool useExternalTargets;

    private string savePath = "Assets/Scripts/StateSystem/Retrievals";

    [MenuItem("Tools/Retrieval Creator")]
    public static void ShowWindow()
    {
        GetWindow<RetrievalCreatorWindow>("Retrieval Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Create a New Retrieval", EditorStyles.boldLabel);

        retrievalName = EditorGUILayout.TextField("Retrieval Name", retrievalName);
        if (!string.IsNullOrEmpty(retrievalName))
        {
            scriptName = $"Retrieve{retrievalName}StateActorScriptableObject";
        }
        else scriptName = "";
        
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("", scriptName);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10);

        useExternalSource = EditorGUILayout.Toggle("Use External Source", useExternalSource);
        EditorGUILayout.Space(5);
        if (!useExternalTargets) useExternalTarget = EditorGUILayout.Toggle("Use External Target", useExternalTarget);
        if (!useExternalTarget) useExternalTargets = EditorGUILayout.Toggle("Use External Target List", useExternalTargets);
        
        EditorGUILayout.Space(15);

        if (File.Exists(Path.Combine(savePath + '/', scriptName + ".cs")))
        {
            GUILayout.Label("Cannot create retrieval: This file already exists at this path.");
        }
        else if (string.IsNullOrEmpty(retrievalName))
        {
            GUILayout.Label("Cannot create retrieval: Retrieval must have a valid name.");
        }
        else if (GUILayout.Button("Create Retrieval"))
        {
            CreateRetrievalScript();
            ResetStateStuff();
        }
    }

    private void ResetStateStuff()
    {
        retrievalName = "";
        scriptName = "";

        useExternalSource = false;
        useExternalTarget = false;
        useExternalTargets = false;
    }

    private void CreateRetrievalScript()
    {
        string members = "";
        if (useExternalSource)
        {
            members += $@"
    [Header(""Source Retrieval"")] 
    public AbstractRetrieveStateActorScriptableObject SourceRetrieval;
";
        }
        if (useExternalTarget)
        {
            members += $@"
    [Header(""Target"")]
    public StateIdentifierTagScriptableObject TargetIdentifier;
";
        }
        else if (useExternalTargets)
        {
            members += $@"
    [Header(""Targets"")]
    public List<StateIdentifierTagScriptableObject> TargetIdentifiers;
";
        }

        string internalLines = "";
        if (useExternalSource)
        {
            internalLines += $@"if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;

";
        }

        if (useExternalTarget)
        {
            internalLines += $@"{(internalLines != string.Empty ? "\t\t" : "")}List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetIdentifier);
        if (targets is null || targets.Count == 0) return null;

";
        }
        else if (useExternalTargets)
        {
            internalLines += $@"{(internalLines != string.Empty ? "\t\t" : "")}if (TargetIdentifiers is null || TargetIdentifiers.Count == 0) return null;

";
        }

        internalLines += $"{(internalLines != string.Empty ? "\t\t" : "")}throw new System.NotImplementedException();";

        string scriptTemplate = $@"using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ""FESState/Retrieval/{retrievalName}"")]
public class {scriptName} : AbstractRetrieveStateActorScriptableObject
{{
    {members}
    public override bool TryRetrieveActor<T>(out T actor)
    {{
        try
        {{
            actor = Retrieve{retrievalName}Actor<T>();
            return actor is not null;
        }}
        catch
        {{
            actor = null;
            return false;
        }}
    }}
    
    public override bool TryRetrieveManyActors<T>(int count, out List<T> actors)
    {{
        try
        {{
            actors = RetrieveMany{retrievalName}Actors<T>(count);
            return actors is not null && actors.Count > 0;
        }}
        catch
        {{
            actors = null;
            return false;
        }}
    }}
    
    public override bool TryRetrieveAllActors<T>(out List<T> actors)
    {{
        try
        {{
            actors = RetrieveMany{retrievalName}Actors<T>(-1);
            return actors is not null && actors.Count > 0;
        }}
        catch
        {{
            actors = null;
            return false;
        }}
    }}
    
    // Implement behaviour here
    private T Retrieve{retrievalName}Actor<T>() where T : StateActor
    {{
        {internalLines}
    }}

    // Implement behaviour here
    // Such that count < 0 should retrieve all actors
    private List<T> RetrieveMany{retrievalName}Actors<T>(int count) where T : StateActor
    {{
        {internalLines}
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
