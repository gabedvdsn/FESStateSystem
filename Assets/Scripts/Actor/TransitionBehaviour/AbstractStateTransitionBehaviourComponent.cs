using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    [RequireComponent(typeof(StateActor))]
    public abstract class AbstractStateTransitionBehaviourComponent<S> : MonoBehaviour where S : MonoBehaviour
    {
        [Header("Matrix")]
        public StateTransitionMatrixScriptableObject TransitionMatrix;

        [Header("Conduits")] 
        public List<AbstractStateTransitionBehaviourConduitScriptableObject<S>> TransitionBehaviourConduits;

        private StateActor State;
        private StateTransitionMatrix<S> Matrix;
        private Dictionary<AbstractStateTransitionBehaviourConduit<S>, bool> RegisteredConduits;

        public delegate void RunAllDelegate(bool interruptStateChange);
        public delegate void RunWithinDelegate(StateContextTagScriptableObject contextTag, bool interruptStateChange);

        private void Awake()
        {
            State = GetComponent<StateActor>();
            
            Matrix = TransitionMatrix.GenerateMatrix(GetComponent<S>(), State);
            foreach (AbstractStateTransitionBehaviourConduitScriptableObject<S> conduit in TransitionBehaviourConduits)
            {
                conduit.InitializeConduit(this);
            }
            
            OnAwakeEvent();
        }

        protected virtual void OnAwakeEvent()
        {
            
        }

        private void OnEnable()
        {
            RunAll();

            OnEnableEvent();
        }

        protected virtual void OnEnableEvent()
        {
            
        }

        #region Transition Running
        
        public void Run(StateTransition<S> transition, StateContextTagScriptableObject contextTag, bool interruptStateChange = true)
        {
            if (!Matrix.TryEvaluateSingleTransition(transition, out TransitionEvaluationResult result)) return;
            PerformTransitionBy(result, contextTag, interruptStateChange);
        }

        public void RunAll(bool interruptStateChange = true)
        {
            Dictionary<StateContextTagScriptableObject, AbstractGameplayState> states = State.Moderator.GetActiveStatesWithContext();
            foreach (StateContextTagScriptableObject contextTag in states.Keys)
            {
                if (!Matrix.TryEvaluateTransitionsFor(states[contextTag].StateData, out TransitionEvaluationResult result)) continue;
                PerformTransitionBy(result, contextTag, interruptStateChange);
            }
        }

        public void SubscribeRunAllTo(ref RunAllDelegate runAllAction) => runAllAction += RunAll;
        public void UnsubscribeRunAllTo(ref RunAllDelegate runAllAction) => runAllAction -= RunAll;

        public void RunWithinContext(StateContextTagScriptableObject contextTag, bool interruptStateChange = true)
        {
            if (!State.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state)) return;
            if (!Matrix.TryEvaluateTransitionsFor(state.StateData, out TransitionEvaluationResult result)) return;
            PerformTransitionBy(result, contextTag, interruptStateChange);
        }

        public void SubscribeRunWithinContextTo(ref RunWithinDelegate runWithinAction) => runWithinAction += RunWithinContext;
        public void UnsubscribeRunWithinContextTo(ref RunWithinDelegate runWithinAction) => runWithinAction -= RunWithinContext;

        private void PerformTransitionBy(TransitionEvaluationResult result, StateContextTagScriptableObject contextTag, bool interrupts = true)
        {
            if (!result.Status) return;
            switch (result.Method)
            {

                case TransitionSelectionMethod.First:
                    if (interrupts) State.Moderator.InterruptChangeState(contextTag, result.First, TransitionMatrix.OnlyDefinedStates);
                    else State.Moderator.DefaultChangeState(contextTag, result.First, TransitionMatrix.OnlyDefinedStates);
                    break;
                case TransitionSelectionMethod.Any:
                    if (interrupts) State.Moderator.InterruptChangeState(contextTag, result.Any, TransitionMatrix.OnlyDefinedStates);
                    else State.Moderator.DefaultChangeState(contextTag, result.Any, TransitionMatrix.OnlyDefinedStates);
                    break;
                case TransitionSelectionMethod.Last:
                    if (interrupts) State.Moderator.InterruptChangeState(contextTag, result.Last, TransitionMatrix.OnlyDefinedStates);
                    else State.Moderator.DefaultChangeState(contextTag, result.Last, TransitionMatrix.OnlyDefinedStates);
                    break;
                case TransitionSelectionMethod.HighPriority:
                    if (interrupts) State.Moderator.InterruptChangeState(contextTag, result.HighestPriority, TransitionMatrix.OnlyDefinedStates);
                    else State.Moderator.DefaultChangeState(contextTag, result.HighestPriority, TransitionMatrix.OnlyDefinedStates);
                    break;
                case TransitionSelectionMethod.LowPriority:
                    if (interrupts) State.Moderator.InterruptChangeState(contextTag, result.LowestPriority, TransitionMatrix.OnlyDefinedStates);
                    else State.Moderator.DefaultChangeState(contextTag, result.LowestPriority, TransitionMatrix.OnlyDefinedStates);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result.Method), result.Method, null);
            }
        }
        
        #endregion
        
        #region Conduit Handling

        public bool RegisterConduit(AbstractStateTransitionBehaviourConduit<S> conduit, bool startOpen = true)
        {
            if (RegisteredConduits.ContainsKey(conduit)) return false;
            RegisteredConduits[conduit] = startOpen;
            return true;
        }

        public bool DeRegisterConduit(AbstractStateTransitionBehaviourConduit<S> conduit)
        {
            if (!ConduitIsRegistered(conduit)) return false;
            RegisteredConduits.Remove(conduit);
            return true;
        }

        public bool CloseConduit(AbstractStateTransitionBehaviourConduit<S> conduit)
        {
            if (!ConduitIsOpen(conduit)) return false;
            RegisteredConduits[conduit] = false;
            return true;
        }
        
        public bool OpenConduit(AbstractStateTransitionBehaviourConduit<S> conduit)
        {
            if (ConduitIsOpen(conduit)) return false;
            RegisteredConduits[conduit] = true;
            return true;
        }

        private bool ConduitIsRegistered(AbstractStateTransitionBehaviourConduit<S> conduit) => RegisteredConduits.ContainsKey(conduit);
        private bool ConduitIsOpen(AbstractStateTransitionBehaviourConduit<S> conduit) => ConduitIsRegistered(conduit) && RegisteredConduits[conduit];

        public bool SendConduitTransition(AbstractStateTransitionBehaviourConduit<S> conduit, LiveTransitionFrequencyData<S> frequencyData)
        {
            if (!ConduitIsOpen(conduit)) return false;
            Run(frequencyData.Transition, frequencyData.Base.Context, frequencyData.Transition.BaseTransition.StateChangeInterrupts);
            return true;
        }
        
        #endregion
    }
}
