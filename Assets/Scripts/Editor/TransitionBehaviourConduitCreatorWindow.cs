using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public class TransitionBehaviourConduitCreatorWindow : EditorWindow
    {
        private string conduitName;
        private string realConduitName;
        private string dataScriptName;
        private string conduitScriptName;
        private MonoScript sourceType;
        private string sourceName;
        private string sourceUsingLine;
        private static bool createData = true;

        private bool attachSourceName;
        private bool attachAsPrefix = true;

        private bool waitingForReload;
        
        private static string savePath = "Assets/Scripts/AuthoredStateSystem/TransitionBehaviourConduits";
        
        [MenuItem("StateSystem/Transition Behaviour/Conduit Creator")]
        public static void ShowWindow()
        {
            GetWindow<TransitionBehaviourConduitCreatorWindow>("Transition Behaviour Conduit Creator");
        }
        
        /// <summary>
        /// We need the monobehaviour that should be encapsulated (sourceType)
        /// From this we deduct the dataScriptName, conduitScriptName, sourceName
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Create a New Transition Behaviour Conduit", EditorStyles.boldLabel);
            
            conduitName = EditorGUILayout.TextField("Conduit Name", conduitName);
            realConduitName = conduitName;
            
            sourceType = (MonoScript)EditorGUILayout.ObjectField(
                "Source Type", 
                sourceType, 
                typeof(MonoScript), 
                false // Set to 'true' if you want to allow selecting objects from the scene
            );

            bool valid = sourceType != null;
            if (valid)
            {
                if (sourceType.GetClass() != null)
                {
                    if (sourceType.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
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
                        
                        EditorGUI.indentLevel = 1;
                        attachSourceName = EditorGUILayout.Toggle("Attach Name", attachSourceName);
                        if (attachSourceName)
                        {
                            attachAsPrefix = EditorGUILayout.Toggle("As Prefix", attachAsPrefix);
                        }
                        
                        if (attachSourceName)
                        {
                            if (attachAsPrefix) realConduitName = $"{sourceName}{conduitName}";
                            else realConduitName = $"{conduitName}{sourceName}";
                        }
                        
                        dataScriptName = $"{realConduitName}TransitionBehaviourConduitScriptableObject";
                        conduitScriptName = $"{realConduitName}TransitionBehaviourConduit";
                
                        savePath = $"Assets/Scripts/AuthoredStateSystem/TransitionBehaviourConduits/{sourceName}";
                    }
                    else valid = false;
                }
                else valid = false;
            }

            if (!valid)
            {
                sourceType = null;
                sourceName = "";
                dataScriptName = "";
                conduitScriptName = "";
                sourceUsingLine = "";
                
                savePath = "Assets/Scripts/AuthoredStateSystem/TransitionBehaviourConduits";
            }
            
            EditorGUI.indentLevel = 0;
        
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("", conduitScriptName);
            EditorGUILayout.TextField("", dataScriptName);
            if (!string.IsNullOrEmpty(sourceUsingLine))
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.TextField("Dependency", $"{sourceUsingLine.Replace(";", "").Replace("using ", "")}");
                EditorGUI.indentLevel = 0;
            }
            EditorGUI.EndDisabledGroup();
        
            EditorGUILayout.Space(10);
            
            // createData = EditorGUILayout.Toggle("Create Data", createData);
            
            GUILayout.Label("Save Path", EditorStyles.boldLabel);
            savePath = EditorGUILayout.TextField("Path", savePath);
            
            EditorGUILayout.Space(15);

            if (File.Exists(Path.Combine(savePath + '/', dataScriptName + ".cs")))
            {
                GUILayout.Label($"Cannot create transition behaviour conduit: {dataScriptName}.cs already exists at this path.");
            }
            else if (File.Exists(Path.Combine(savePath + '/', conduitScriptName + ".cs")))
            {
                GUILayout.Label($"Cannot create transition behaviour conduit: {conduitScriptName}.cs already exists at this path.");
            }
            else if (string.IsNullOrEmpty(realConduitName))
            {
                GUILayout.Label($"Cannot create transition behaviour conduit: Must have a valid conduit name.");
            }
            else if (sourceType is null)
            {
                GUILayout.Label("Cannot create transition behaviour conduit: Must have a valid source type.");
            }
            else if (GUILayout.Button("Create Transition Behaviour Conduit"))
            {
                CreateTransitionBehaviourConduitScripts();
                ResetStateStuff();
            }
        }

        private void ResetStateStuff()
        {
            conduitName = "";
            realConduitName = "";
            
            sourceType = null;
            sourceName = "";
            dataScriptName = "";
            conduitScriptName = "";
            sourceUsingLine = "";

            attachSourceName = false;
            attachAsPrefix = true;
                
            savePath = "Assets/Scripts/AuthoredStateSystem/TransitionBehaviourConduits";
        }

        private void CreateAbstractScript()
        {
            string usingStatements = $"using UnityEngine;\nusing FESStateSystem;\nusing System.Collections.Generic;\nusing AYellowpaper.SerializedCollections;";
            if (!string.IsNullOrEmpty(sourceUsingLine)) usingStatements += $"\n{sourceUsingLine}";
            
            string enumTemplate = $@"
public enum {sourceName}Frequency
{{

}}
";
            
            string abstractConduitTemplate = $@"{usingStatements}

public abstract class Abstract{sourceName}TransitionBehaviourConduit : AbstractTransitionBehaviourConduit<{sourceName}>
{{
    [SerializedDictionary(""Frequency"", ""Transition Data"")]
    [SerializeField] private SerializedDictionary<{sourceName}Frequency, TransitionFrequencyData> TransitionFrequencies;

    private Dictionary<{sourceName}Frequency, LiveFrequencyTransitionData<{sourceName}>> LiveTransitionFrequencies;

    protected override void InitializeFrequencies()
    {{
        LiveTransitionFrequencies = new Dictionary<{sourceName}Frequency, LiveFrequencyTransitionData<{sourceName}>>();

        foreach ({sourceName}Frequency frequency in TransitionFrequencies.Keys)
        {{
            LiveTransitionFrequencies[frequency] = TransitionFrequencies[frequency].ToLiveData<{sourceName}>();
        }}
    }}

    public void Transmit({sourceName}Frequency frequency)
    {{
        TransmitOn(frequency);
    }}
    
    protected bool TransmitOn({sourceName}Frequency frequency)
    {{
        if (!RecipientComponent) return false;
        return TryGetTransitionOn(frequency, out LiveFrequencyTransitionData<{sourceName}> frequencyData) && RecipientComponent.SendConduitTransition(this, frequencyData);
    }}

    private bool TryGetTransitionOn({sourceName}Frequency frequency, out LiveFrequencyTransitionData<{sourceName}> frequencyData)
    {{
        return LiveTransitionFrequencies.TryGetValue(frequency, out frequencyData);
    }}
    
  
}}

{enumTemplate}
";
            string folderPath = savePath + "/";
            string conduitFilepath = Path.Combine(folderPath, $"Abstract{sourceName}TransitionBehaviourConduit.cs");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Write the script to the file
            File.WriteAllText(conduitFilepath, abstractConduitTemplate);
            
        }

        private void CreateTransitionBehaviourConduitScripts()
        {
            if (!ScriptUtils.DoesScriptExist($"Abstract{sourceName}TransitionBehaviourConduit")) CreateAbstractScript();
            
            string usingStatements = $"using UnityEngine;\nusing FESStateSystem;";
            if (!string.IsNullOrEmpty(sourceUsingLine)) usingStatements += $"\n{sourceUsingLine}";
            string dataScriptTemplate = $@"{usingStatements}

[CreateAssetMenu(menuName = ""FESState/Authored/Transition Behaviour Conduit/{sourceName}/{conduitName}"", fileName = ""{realConduitName}Conduit"")]
public class {dataScriptName} : AbstractTransitionBehaviourConduitScriptableObject<{sourceName}>
{{
    [Header(""Conduit Prefab"")]
    public {conduitScriptName} ConduitPrefab;  // DO NOT change the name of this field
    
    protected override AbstractTransitionBehaviourConduit<{sourceName}> CreateConduit(AbstractTransitionBehaviourComponent<{sourceName}> transitionComponent)
    {{
        if (!ConduitPrefab) return null;

        {conduitScriptName} conduit = InstantiateConduit(ConduitPrefab, transitionComponent.transform);

        // Write additional logic

        return conduit;
    }}
}}

";
            
            string conduitScriptTemplate = $@"{usingStatements}

public class {conduitScriptName} : Abstract{sourceName}TransitionBehaviourConduit
{{

}}

";


            string folderPath = savePath + "/" + conduitName + "/";
            string dataFilepath = Path.Combine(folderPath, dataScriptName + ".cs");
            string conduitFilepath = Path.Combine(folderPath, conduitScriptName + ".cs");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Write the script to the file
            File.WriteAllText(dataFilepath, dataScriptTemplate);
            File.WriteAllText(conduitFilepath, conduitScriptTemplate);
            
            // Refresh the Asset Database to show the new script
            AssetDatabase.Refresh();

            /*if (createData)
            {
                waitingForReload = true;
                EditorApplication.update += WaitForCompilationAndCreateData;
            }*/
        }
    }
}
