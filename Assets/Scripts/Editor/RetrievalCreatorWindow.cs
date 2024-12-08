using System.IO;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public class RetrievalCreatorWindow : EditorWindow
    {
        private string retrievalName = "";
        private string scriptName = "";
        private bool useExternalSource;
        private bool useExternalTarget;
        private bool useExternalTargets;

        private string savePath = "Assets/Scripts/AuthoredStateSystem/Retrievals";

        [MenuItem("StateSystem/Retrieval Creator")]
        public static void ShowWindow()
        {
            GetWindow<RetrievalCreatorWindow>("Retrieval Creator");
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
        }

        private void OnGUI()
        {
            GUILayout.Label("Create a New Retrieval", EditorStyles.boldLabel);

            retrievalName = EditorGUILayout.TextField("Retrieval Name", retrievalName);
            if (!string.IsNullOrEmpty(retrievalName))
            {
                scriptName = $"Retrieve{retrievalName.Replace(" ", "")}StateActorScriptableObject";
            }
            else scriptName = "";
        
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", scriptName);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);

            useExternalSource = EditorGUILayout.Toggle("Use External Source", useExternalSource);
            EditorGUILayout.Space(5);
            if (!useExternalTargets) useExternalTarget = EditorGUILayout.Toggle("Use External Target", useExternalTarget);
            if (!useExternalTarget) useExternalTargets = EditorGUILayout.Toggle("Use External Target List", useExternalTargets);
        
            EditorGUILayout.Space(10);
            GUILayout.Label("Save Path", EditorStyles.boldLabel);

            savePath = EditorGUILayout.TextField("Path", savePath);
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

            string retrieveActor = "";
            string retrieveManyActors = "";
            if (useExternalSource)
            {
                retrieveActor += $@"if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;

";
                retrieveManyActors += $@"if (!SourceRetrieval.TryRetrieveManyActors(count, out List<StateActor> sources)) return null;

";
            }

            if (useExternalTarget)
            {
                retrieveActor += $@"{(retrieveActor != string.Empty ? "\t\t" : "")}List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetIdentifier);
        if (targets is null || targets.Count == 0) return null;

";
                retrieveManyActors += $@"{(retrieveActor != string.Empty ? "\t\t" : "")}List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetIdentifier);
        if (targets is null || targets.Count == 0) return null;

";
            }
            else if (useExternalTargets)
            {
                retrieveActor += $@"{(retrieveActor != string.Empty ? "\t\t" : "")}if (TargetIdentifiers is null || TargetIdentifiers.Count == 0) return null;

";
                retrieveManyActors += $@"{(retrieveActor != string.Empty ? "\t\t" : "")}if (TargetIdentifiers is null || TargetIdentifiers.Count == 0) return null;

";
            }

            retrieveActor += $"{(retrieveActor != string.Empty ? "\t\t" : "")}throw new System.NotImplementedException();";
            retrieveManyActors += $"{(retrieveManyActors != string.Empty ? "\t\t" : "")}throw new System.NotImplementedException();";

            string scriptTemplate = $@"using System.Collections.Generic;
using UnityEngine;
using FESStateSystem;

[CreateAssetMenu(menuName = ""FESState/Authored/Retrieval/{retrievalName}"")]
public class {scriptName} : AbstractRetrieveStateActorScriptableObject
{{
    {members}
    // Implement behaviour here
    protected override T RetrieveActor<T>()
    {{
        {retrieveActor}
    }}

    // Implement behaviour here
    // Such that count < 0 should retrieve all actors
    protected override List<T> RetrieveManyActors<T>(int count)
    {{
        {retrieveManyActors}
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