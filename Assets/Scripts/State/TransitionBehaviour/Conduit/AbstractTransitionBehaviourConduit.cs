using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    public abstract class AbstractTransitionBehaviourConduit<S> : BaseAbstractTransitionBehaviourConduit, ITransitionBehaviourConduit where S : MonoBehaviour
    {
        [SerializedDictionary("Frequency", "Transitions")]
        public SerializedDictionary<StateConduitFrequencyTagScriptableObject, TransitionFrequencyData> TransitionFrequencies;

        private Dictionary<StateConduitFrequencyTagScriptableObject, LiveFrequencyTransitionData<S>> LiveTransitionFrequencies;
        private AbstractTransitionBehaviourComponent<S> RecipientComponent;
        
        public void Initialize(AbstractTransitionBehaviourComponent<S> transitionComponent, bool conduitStartsOpen)
        {
            RecipientComponent = transitionComponent;
            RecipientComponent.RegisterConduit(this, conduitStartsOpen);
            
            LiveTransitionFrequencies = new Dictionary<StateConduitFrequencyTagScriptableObject, LiveFrequencyTransitionData<S>>();
            foreach (StateConduitFrequencyTagScriptableObject frequencyTag in TransitionFrequencies.Keys)
            {
                LiveTransitionFrequencies[frequencyTag] = TransitionFrequencies[frequencyTag].ToLiveData<S>();
            }

            gameObject.name += $" ({RecipientComponent.name})";
        }

        public AbstractTransitionBehaviourConduit<S1> Get<S1>() where S1 : MonoBehaviour
        {
            return this as AbstractTransitionBehaviourConduit<S1>;
        }
        
        protected bool TransmitOn(StateConduitFrequencyTagScriptableObject frequency)
        {
            if (!RecipientComponent) return false;
            return TryGetTransitionOn(frequency, out LiveFrequencyTransitionData<S> frequencyData) && RecipientComponent.SendConduitTransition(this, frequencyData);
        }

        private bool TryGetTransitionOn(StateConduitFrequencyTagScriptableObject frequency, out LiveFrequencyTransitionData<S> frequencyData)
        {
            return LiveTransitionFrequencies.TryGetValue(frequency, out frequencyData);
        }

        public void ForceTransmitAll()
        {
            if (!RecipientComponent) return;
            foreach (StateConduitFrequencyTagScriptableObject frequencyTag in LiveTransitionFrequencies.Keys)
            {
                TransmitOn(frequencyTag);
            }
        }
        
        public void CleanDependencies()
        {
            
        }
    }

    [Serializable]
    public class TransitionFrequencyData
    {
        public StateContextTagScriptableObject Context;
        public StateTransitionScriptableObject Transition;

        public LiveFrequencyTransitionData<S> ToLiveData<S>() where S : MonoBehaviour
        {
            return new LiveFrequencyTransitionData<S>()
            {
                Base = this,
                Transition = StateTransition<S>.Generate(Transition)
            };
        }

        public static TransitionFrequencyData Generate(StateContextTagScriptableObject context, StateTransitionScriptableObject transition)
        {
            return new TransitionFrequencyData()
            {
                Context = context,
                Transition = transition
            };
        }
    }

    public class LiveFrequencyTransitionData<S> where S : MonoBehaviour
    {
        public TransitionFrequencyData Base;
        public StateTransition<S> Transition;
    }
}
