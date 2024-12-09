using System;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    [RequireComponent(typeof(StateActor))]
    public abstract class AbstractStateTransitionComponent<S> : MonoBehaviour where S : MonoBehaviour
    {
        public StateTransitionMatrixScriptableObject TransitionMatrix;

        [Space] 
        
        public bool Log;
        
        protected StateActor State;
        protected StateTransitionMatrix<S> Matrix;

        public delegate void RunAllDelegate(bool interruptStateChange);
        public delegate void RunForDelegate(StateContextTagScriptableObject contextTag, bool interruptStateChange);

        private void Awake()
        {
            State = GetComponent<StateActor>();
            Matrix = TransitionMatrix.GenerateMatrix(GetComponent<S>());
            
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

        public void RunAll(bool interruptStateChange = true)
        {
            System.Collections.Generic.Dictionary<StateContextTagScriptableObject, AbstractGameplayState> states = State.Moderator.GetActiveStatesWithPriority();
            foreach (StateContextTagScriptableObject priorityTag in states.Keys)
            {
                if (!Matrix.TryEvaluateTransitionsFor(states[priorityTag].StateData, out TransitionEvaluationResult result)) continue;
                PerformTransitionBy(result, priorityTag, interruptStateChange);
            }
        }

        public void SubscribeRunAllTo(ref RunAllDelegate runAllAction) => runAllAction += RunAll;
        public void UnsubscribeRunAllTo(ref RunAllDelegate runAllAction) => runAllAction -= RunAll;

        public void RunForPriority(StateContextTagScriptableObject contextTag, bool interruptStateChange = true)
        {
            if (!State.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state)) return;
            if (!Matrix.TryEvaluateTransitionsFor(state.StateData, out TransitionEvaluationResult result)) return;
            PerformTransitionBy(result, contextTag, interruptStateChange);
        }

        public void SubscribeRunForPriorityTo(ref RunForDelegate runForAction) => runForAction += RunForPriority;
        public void UnsubscribeRunForPriorityTo(ref RunForDelegate runForAction) => runForAction -= RunForPriority;

        private void PerformTransitionBy(TransitionEvaluationResult result, StateContextTagScriptableObject contextTag, bool interrupts = true)
        {
            if (!result.Status) return;
            switch (result.Method)
            {

                case TransitionSelectionMethod.First:
                    State.Moderator.InterruptChangeState(contextTag, result.First, interrupts);
                    break;
                case TransitionSelectionMethod.Any:
                    State.Moderator.InterruptChangeState(contextTag, result.Any, interrupts);
                    break;
                case TransitionSelectionMethod.Last:
                    State.Moderator.InterruptChangeState(contextTag, result.Last, interrupts);
                    break;
                case TransitionSelectionMethod.HighPriority:
                    State.Moderator.InterruptChangeState(contextTag, result.HighestPriority, interrupts);
                    break;
                case TransitionSelectionMethod.LowPriority:
                    State.Moderator.InterruptChangeState(contextTag, result.LowestPriority, interrupts);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result.Method), result.Method, null);
            }
        }
    }
}
