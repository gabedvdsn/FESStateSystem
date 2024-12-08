using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Manifest")]
    public class GameplayStateManifestScriptableObject : ScriptableObject
    {
        [Tooltip("The first entry for each context tag is each context's initial state.")]
        [SerializedDictionary("Context Tag", "Permitted States")]
        public SerializedDictionary<StateContextTagScriptableObject, List<AbstractGameplayStateBehaviourScriptableObject>>
            ContextManifest;

        public StateContextTagScriptableObject[] Contexts => ContextManifest.Keys.ToArray();

        public AbstractGameplayStateScriptableObject InitialState(StateContextTagScriptableObject contextTag) =>
            ContextManifest[contextTag] != null && ContextManifest[contextTag].Count > 0 ? ContextManifest[contextTag][0].Initial() : null;

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

    }
}