using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Manifest")]
    public class GameplayStateManifestScriptableObject : ScriptableObject
    {
        [SerializedDictionary("Context Tag", "Initial State")]
        public SerializedDictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject>
            InitialStates;
        
        [SerializedDictionary("Context Tag", "Permitted States")]
        public SerializedDictionary<StateContextTagScriptableObject, List<AbstractGameplayStateBehaviourScriptableObject>>
            ContextManifest;

        public StateContextTagScriptableObject[] Contexts => ContextManifest.Keys.ToArray();

        public AbstractGameplayStateScriptableObject InitialState(StateContextTagScriptableObject contextTag) => InitialStates[contextTag];

        public List<AbstractGameplayStateScriptableObject> Get(StateContextTagScriptableObject contextTag)
        {
            List<AbstractGameplayStateScriptableObject> states = new List<AbstractGameplayStateScriptableObject>();
            foreach (AbstractGameplayStateBehaviourScriptableObject stateBehaviour in ContextManifest[contextTag])
            {
                states.AddRange(stateBehaviour.Get());
            }

            return states;
        }

        public bool DefinesState(AbstractGameplayStateScriptableObject state)
        {
            return Contexts.Any(context => ContextManifest[context].Any(stateBehaviour => stateBehaviour.Defines(state)));
        }

        public bool DefinesState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject state)
        {
            return ContextManifest[contextTag].Any(stateBehaviour => stateBehaviour.Defines(state));
        }

        private void OnValidate()
        {
            foreach (StateContextTagScriptableObject contextTag in InitialStates.Keys)
            {
                if (!ContextManifest.ContainsKey(contextTag)) throw new Exception($"[ {name} ] Initial state context {contextTag.name} is missing from manifest");
                if (InitialStates[contextTag] != null && !ContextManifest[contextTag].Any(stateBehaviour => stateBehaviour.Defines(InitialStates[contextTag])))
                    throw new Exception($"[ {name} ] Initial state {InitialStates[contextTag].name} is missing from manifest under {contextTag.name}");
                if (InitialStates[contextTag] is null) throw new Exception($"[ {name} ] Missing initial state under {contextTag.name}");
            }

            foreach (StateContextTagScriptableObject contextTag in ContextManifest.Keys)
            {
                if (!InitialStates.ContainsKey(contextTag)) throw new Exception($"[ {name} ] Missing context {contextTag.name} in initial states");
            }
        }

    }
}