using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    public abstract class AbstractStateTransitionBehaviourConduit<S> : MonoBehaviour, ITransitionBehaviourConduit where S : MonoBehaviour
    {
        [SerializedDictionary("Frequency", "Transitions")]
        public SerializedDictionary<StateConduitFrequencyTagScriptableObject, TransitionFrequencyData> TransitionFrequencies;

        private Dictionary<StateConduitFrequencyTagScriptableObject, LiveTransitionFrequencyData<S>> LiveTransitionFrequencies;
        private AbstractStateTransitionBehaviourComponent<S> RecipientComponent;
        
        public void Initialize(AbstractStateTransitionBehaviourComponent<S> transitionComponent, bool conduitStartsOpen)
        {
            RecipientComponent = transitionComponent;
            RecipientComponent.RegisterConduit(this, conduitStartsOpen);
            
            LiveTransitionFrequencies = new Dictionary<StateConduitFrequencyTagScriptableObject, LiveTransitionFrequencyData<S>>();
            foreach (StateConduitFrequencyTagScriptableObject frequencyTag in TransitionFrequencies.Keys)
            {
                LiveTransitionFrequencies[frequencyTag] = TransitionFrequencies[frequencyTag].ToLiveData<S>();
            }
        }
        
        protected bool TransmitOn(StateConduitFrequencyTagScriptableObject frequency)
        {
            if (!RecipientComponent) return false;
            return TryGetTransitionOn(frequency, out LiveTransitionFrequencyData<S> frequencyData) && RecipientComponent.SendConduitTransition(this, frequencyData);
        }

        private bool TryGetTransitionOn(StateConduitFrequencyTagScriptableObject frequency, out LiveTransitionFrequencyData<S> frequencyData)
        {
            return LiveTransitionFrequencies.TryGetValue(frequency, out frequencyData);
        }
        
        public void Clean()
        {
            
        }
    }

    [Serializable]
    public class TransitionFrequencyData
    {
        public StateContextTagScriptableObject Context;
        public StateTransitionScriptableObject Transition;

        public LiveTransitionFrequencyData<S> ToLiveData<S>() where S : MonoBehaviour
        {
            return new LiveTransitionFrequencyData<S>()
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

    public class LiveTransitionFrequencyData<S> where S : MonoBehaviour
    {
        public TransitionFrequencyData Base;
        public StateTransition<S> Transition;
    }
}
