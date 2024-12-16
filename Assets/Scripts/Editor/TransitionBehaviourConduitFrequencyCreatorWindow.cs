using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEngine;

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
        
        [MenuItem("StateSystem/Transition Behaviour/Conduit Frequency Creator")]
        public static void ShowWindow()
        {
            GetWindow<TransitionBehaviourConduitFrequencyCreatorWindow>("Conduit Frequency Creator");
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            serializedListProperty = serializedObject.FindProperty("enumValues");
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

                // Display the list in the Editor window
                EditorGUILayout.PropertyField(serializedListProperty, new GUIContent("Frequency Values"), true);
            
                if (GUILayout.Button("Add Frequency"))
                {
                    enumValues.Add("New Frequency");
                }
            
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
                else
                {
                    GUILayout.Label("Cannot create transition behaviour conduit frequencies: Please create a conduit script first via 'State System/Transition Behaviour/Conduit Creator.'");
                }
            }
            
            
        }

        private void ApplyEnumChanges()
        {
            
            
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            ResetStuff();
        }

        private void GetEnumValues()
        {
            if (!ScriptUtils.DoesScriptExist($"{sourceName}"))
            {
                valid = false;
                enumValues = null;
                return;
            }

            string file = ScriptUtils.GetFileContents($"{sourceName}");
            string enumName = $"{sourceName}".Replace("Abstract", "");
            enumName = enumName.Replace("TransitionBehaviourConduit", "");
            int enumIndex = file.LastIndexOf($"{enumName}Frequency", StringComparison.InvariantCulture);
            string enumString = file.Substring(enumIndex);
            enumString = enumString.Replace("{", "");
            enumString = enumString.Replace("}", "");
            enumString = enumString.Replace($"{sourceName}Frequency", "");
            enumString = enumString.Replace($"{enumName}Frequency", "");
            enumString = enumString.Replace("\n", "");
            string[] enumStrings = enumString.Split(',');

            enumValues = enumStrings.ToList();
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
