using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace FESStateSystem
{
    public class TransitionBehaviourConduitFrequencyCreatorWindow : EditorWindow
    {
        private MonoScript sourceType;
        private string sourceName;
        
        [SerializeField]
        private List<string> enumValues = new List<string>();
        
        private SerializedObject serializedObject;
        private SerializedProperty serializedListProperty;

        private bool valid = false;
        private bool gotResults;
        private bool validSourceFile;

        private string sourceFilePath;
        private string sourceFile;
        private int enumIndex;
        private string sourceEnumString;
        
        [MenuItem("StateSystem/Transition Behaviour/Conduit Frequency Creator")]
        public static void ShowWindow()
        {
            GetWindow<TransitionBehaviourConduitFrequencyCreatorWindow>("Conduit Frequency Creator");
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            serializedListProperty = serializedObject.FindProperty("enumValues");
            
            ResetStuff();
        }

        /// <summary>
        /// We need the monobehaviour that should be encapsulated (sourceType)
        /// From this we deduct the dataScriptName, conduitScriptName, sourceName
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Create New Transition Behaviour Conduit Frequencies", EditorStyles.boldLabel);
            
            sourceType = (MonoScript)EditorGUILayout.ObjectField(
                "Source Type", 
                sourceType, 
                typeof(MonoScript), 
                false // Set to 'true' if you want to allow selecting objects from the scene
            );

            if (sourceType is not null)
            {
                if (!typeof(ITransitionBehaviourConduit).IsAssignableFrom(sourceType.GetClass()))
                {
                    sourceType = null;
                }
                else if (!gotResults)
                {
                    sourceName = sourceType.GetClass().ToString().Split('.')[^1];
                    GetEnumValues();
                }
            }
            else
            {
                valid = false;
                gotResults = false;
            }

            
            if (valid)
            {
                serializedObject.Update();
                
                string frequencyName = $"{sourceName}".Replace("Abstract", "");
                frequencyName = frequencyName.Replace("TransitionBehaviourConduit", "");
                // Display the list in the Editor window
                EditorGUILayout.PropertyField(serializedListProperty, new GUIContent($"{frequencyName} Frequencies [Duplicates will be removed]"), true);
            
                // Apply changes made to the list
                serializedObject.ApplyModifiedProperties();
                
                if (sourceType is null)
                {
                    GUILayout.Label("Cannot create transition behaviour conduit frequencies: Must have a valid source type.");
                }
                else if (GUILayout.Button("Create Frequencies"))
                {
                    ApplyEnumChanges();
                    ResetStuff();
                }
            }
            else
            {
                if (sourceType is null)
                {
                    GUILayout.Label("Cannot create transition behaviour conduit frequencies: Must have a valid source type");
                }
                else if (!validSourceFile)
                {
                    GUILayout.Label("Cannot create transition behaviour conduit frequencies: Invalid choice of source type. Must be a derived conduit. Please create a derived conduit script via 'State System/Transition Behaviour/Conduit Creator.'");
                }
                else
                {
                    GUILayout.Label("Cannot create transition behaviour conduit frequencies: Please create a conduit script first via 'State System/Transition Behaviour/Conduit Creator.'");
                }
            }
            
            
        }

        private void ApplyEnumChanges()
        {
            string newEnumString = $"{sourceName}".Replace("Abstract", "").Replace("TransitionBehaviourConduit", "");
            newEnumString += "Frequency";
            
            newEnumString += "\n{";
            List<string> newValues = new List<string>();
            foreach (string value in enumValues)
            {
                string _value = Regex.Replace(value, @"\s+", "");
                
                if (newValues.Contains(_value)) continue;
                newValues.Add(_value);
            }

            int count = 0;
            foreach (string value in newValues)
            {
                newEnumString += $"\n\t{value}";
                if (count++ < newValues.Count - 1) newEnumString += ",";
            }

            newEnumString += "\n}";

            sourceFile = sourceFile.Replace(sourceEnumString, newEnumString);

            File.WriteAllText(sourceFilePath, sourceFile);
            
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            ResetStuff();
        }

        private void GetEnumValues()
        {
            if (!ScriptUtils.DoesScriptExist($"{sourceName}", out sourceFilePath))
            {
                valid = false;
                enumValues = null;
                return;
            }

            sourceFile = ScriptUtils.GetFileContents($"{sourceName}");
            string enumName = $"{sourceName}".Replace("Abstract", "");
            enumName = enumName.Replace("TransitionBehaviourConduit", "");
            
            enumIndex = sourceFile.LastIndexOf($"{enumName}Frequency", StringComparison.InvariantCulture);
            if (enumIndex < 0) return;

            validSourceFile = true;
            
            sourceEnumString = sourceFile.Substring(enumIndex);

            string enumString = sourceEnumString;
            enumString = enumString.Replace("{", "");
            enumString = enumString.Replace("}", "");
            enumString = enumString.Replace($"{sourceName}Frequency", "");
            enumString = enumString.Replace($"{enumName}Frequency", "");
            enumString = enumString.Replace("\n", "");
            enumString = Regex.Replace(enumString, @"\s+", "");
            
            string[] enumStrings = enumString.Split(',');

            enumValues = new List<string>();
            foreach (string enumValue in enumStrings)
            {
                if (!string.IsNullOrEmpty(enumValue)) enumValues.Add(enumValue);
            }
            gotResults = true;
            valid = true;
        }

        private void ResetStuff()
        {
            sourceType = null;
            valid = false;
            sourceName = "";
            gotResults = false;
        }
    }
}
