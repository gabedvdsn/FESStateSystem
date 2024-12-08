using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using JetBrains.Annotations;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Environment")]
    public class StateEnvironmentScriptableObject : ScriptableObject
    {
        [SerializedDictionary("General Identifier Tag", "Initial State Trigger")]
        [SerializeField] private SerializedDictionary<StateIdentifierTagScriptableObject, InitializationStateTriggerScriptableObject> GeneralIdentifierInitialStateTriggers;

        public InitializationStateTriggerScriptableObject GetInitialStateTrigger(StateIdentifierTagScriptableObject GeneralIdentifierTag)
        {
            return GeneralIdentifierInitialStateTriggers.TryGetValue(GeneralIdentifierTag, out InitializationStateTriggerScriptableObject initialTrigger) ? initialTrigger : null;
        }
    }
}