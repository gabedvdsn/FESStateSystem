using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    [RequireComponent(typeof(StateActor))]
    public abstract class AbstractTransitionBehaviourComponent<S> : MonoBehaviour where S : MonoBehaviour
    {
        [Header("Matrix")]
        public StateTransitionMatrixScriptableObject TransitionMatrix;

        [Header("Conduits")] 
        public List<AbstractTransitionBehaviourConduitScriptableObject<S>> TransitionBehaviourConduits;

        private StateActor State;
        private StateTransitionMatrix<S> Matrix;
        private Dictionary<AbstractTransitionBehaviourConduit<S>, bool> RegisteredConduits;

        public delegate void RunAllDelegate(bool interruptStateChange);
        public delegate void RunWithinDelegate(StateContextTagScriptableObject contextTag, bool interruptStateChange);

        private void Awake()
        {
            State = GetComponent<StateActor>();

            if (TransitionMatrix)
            {
                Matrix = TransitionMatrix.GenerateMatrix(GetComponent<S>(), State);
                foreach (AbstractTransitionBehaviourConduitScriptableObject<S> conduit in TransitionBehaviourConduits)
                {
                    conduit.InitializeConduit(this);
                }
            }
            
            OnAwakeEvent();
        }

        protected virtual void OnAwakeEvent()
        {
            
        }

        private void Start()
        {
            RunAll();

            OnStartEvent();
        }

        protected virtual void OnStartEvent()
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

        public void RunWithinContext(StateContextTagScriptableObject contextTag, bool interruptStateChange = true)
        {
            if (!State.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state)) return;
            if (!Matrix.TryEvaluateTransitionsFor(state.StateData, out TransitionEvaluationResult result)) return;
            PerformTransitionBy(result, contextTag, interruptStateChange);
        }

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

        public bool RegisterConduit(AbstractTransitionBehaviourConduit<S> conduit, bool startOpen = true)
        {
            RegisteredConduits ??= new Dictionary<AbstractTransitionBehaviourConduit<S>, bool>();
            
            if (ConduitIsRegistered(conduit)) return false;
            RegisteredConduits[conduit] = startOpen;
            return true;
        }

        public bool DeRegisterConduit(AbstractTransitionBehaviourConduit<S> conduit)
        {
            if (!ConduitIsRegistered(conduit)) return false;
            RegisteredConduits.Remove(conduit);
            return true;
        }

        public bool CloseConduit(AbstractTransitionBehaviourConduit<S> conduit)
        {
            if (!ConduitIsOpen(conduit)) return false;
            RegisteredConduits[conduit] = false;
            return true;
        }
        
        public bool OpenConduit(AbstractTransitionBehaviourConduit<S> conduit)
        {
            if (ConduitIsOpen(conduit)) return false;
            RegisteredConduits[conduit] = true;
            return true;
        }

        private bool ConduitIsRegistered(AbstractTransitionBehaviourConduit<S> conduit) => RegisteredConduits.ContainsKey(conduit);
        private bool ConduitIsOpen(AbstractTransitionBehaviourConduit<S> conduit) => ConduitIsRegistered(conduit) && RegisteredConduits[conduit];

        public bool SendConduitTransition(AbstractTransitionBehaviourConduit<S> conduit, LiveFrequencyTransitionData<S> frequencyData)
        {
            if (!ConduitIsOpen(conduit)) return false;
            Run(frequencyData.Transition, frequencyData.Base.Context, frequencyData.Transition.BaseTransition.StateChangeInterrupts);
            return true;
        }
        
        #endregion
    }
}
