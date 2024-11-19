using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Environment")]
public class StateEnvironmentScriptableObject : ScriptableObject
{
    [SerializedDictionary("General Identifier Tag", "Initial State Trigger")]
    [SerializeField] private SerializedDictionary<GameplayStateTagScriptableObject, InitializationStateTriggerScriptableObject> GeneralIdentifierInitialStateTriggers;

    public InitializationStateTriggerScriptableObject GetInitialStateTrigger(GameplayStateTagScriptableObject GeneralIdentifierTag)
    {
        return GeneralIdentifierInitialStateTriggers.TryGetValue(GeneralIdentifierTag, out InitializationStateTriggerScriptableObject initialTrigger) ? initialTrigger : null;
    }
}