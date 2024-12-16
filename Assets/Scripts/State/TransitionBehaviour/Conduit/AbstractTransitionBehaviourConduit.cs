using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    public abstract class AbstractTransitionBehaviourConduit<S> : MonoBehaviour, ITransitionBehaviourConduit where S : MonoBehaviour
    {
        protected AbstractTransitionBehaviourComponent<S> RecipientComponent;
                
        public void Initialize(AbstractTransitionBehaviourComponent<S> transitionComponent, bool conduitStartsOpen)
        {
            RecipientComponent = transitionComponent;
            RecipientComponent.RegisterConduit(this, conduitStartsOpen);
            
            InitializeFrequencies();
        }

        protected abstract void InitializeFrequencies();

        /*public void Transmit(StateConduitFrequencyTagScriptableObject frequency)
        {
            TransmitOn(frequency);
        }*/
        
        /*protected bool TransmitOn(StateConduitFrequencyTagScriptableObject frequency)
        {
            if (!RecipientComponent) return false;
            return TryGetTransitionOn(frequency, out LiveFrequencyTransitionData<S> frequencyData) && RecipientComponent.SendConduitTransition(this, frequencyData);
        }*/

        /*private bool TryGetTransitionOn(StateConduitFrequencyTagScriptableObject frequency, out LiveFrequencyTransitionData<S> frequencyData)
        {
            return LiveTransitionFrequencies.TryGetValue(frequency, out frequencyData);
        }*/

        public void ForceTransmitAll()
        {
            /*if (!RecipientComponent) return;
            foreach (StateConduitFrequencyTagScriptableObject frequencyTag in LiveTransitionFrequencies.Keys)
            {
                TransmitOn(frequencyTag);
            }*/
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
    }

    public class LiveFrequencyTransitionData<S> where S : MonoBehaviour
    {
        public TransitionFrequencyData Base;
        public StateTransition<S> Transition;
    }
}
