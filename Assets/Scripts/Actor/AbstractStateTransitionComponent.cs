using System;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    [RequireComponent(typeof(StateActor))]
    public abstract class AbstractStateTransitionComponent<S> : MonoBehaviour
    {
        public StateTransitionMatrixScriptableObject TransitionMatrix;

        [Space] 
        
        public bool Log;
        
        protected StateActor State;
        protected StateTransitionMatrix<S> Matrix;

        public delegate void RunAllDelegate();
        public delegate void RunForDelegate(StateContextTagScriptableObject contextTag);

        private void Awake()
        {
            State = GetComponent<StateActor>();
            InitializeMatrix();
            
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        public void RunAll()
        {
            System.Collections.Generic.Dictionary<StateContextTagScriptableObject, AbstractGameplayState> states = State.Moderator.GetActiveStatesWithPriority();
            foreach (StateContextTagScriptableObject priorityTag in states.Keys)
            {
                if (!Matrix.TryEvaluateTransitionsFor(states[priorityTag].StateData, out TransitionEvaluationResult result)) continue;
                PerformTransitionBy(result, priorityTag);
            }
        }

        public void SubscribeRunAllTo(ref RunAllDelegate runAllAction) => runAllAction += RunAll;
        public void UnsubscribeRunAllTo(ref RunAllDelegate runAllAction) => runAllAction -= RunAll;

        public void RunForPriority(StateContextTagScriptableObject contextTag)
        {
            if (!State.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state)) return;
            if (!Matrix.TryEvaluateTransitionsFor(state.StateData, out TransitionEvaluationResult result)) return;
            PerformTransitionBy(result, contextTag);
        }

        public void SubscribeRunForPriorityTo(ref RunForDelegate runForAction) => runForAction += RunForPriority;
        public void UnsubscribeRunForPriorityTo(ref RunForDelegate runForAction) => runForAction -= RunForPriority;

        private void PerformTransitionBy(TransitionEvaluationResult result, StateContextTagScriptableObject contextTag)
        {
            if (!result.Status) return;
            switch (result.Method)
            {

                case TransitionEvaluationSelect.First:
                    State.Moderator.InterruptChangeState(contextTag, result.First());
                    break;
                case TransitionEvaluationSelect.Any:
                    State.Moderator.InterruptChangeState(contextTag, result.Any());
                    break;
                case TransitionEvaluationSelect.Last:
                    State.Moderator.InterruptChangeState(contextTag, result.Last());
                    break;
                case TransitionEvaluationSelect.Select:
                    State.Moderator.InterruptChangeState(contextTag, GetSelectTransition(contextTag, result));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result.Method), result.Method, null);
            }
        }

        protected virtual AbstractGameplayStateScriptableObject GetSelectTransition(StateContextTagScriptableObject contextTag, TransitionEvaluationResult result)
        {
            return !result.Status ? null : result.First();
        }

        /// <summary>
        /// Initialize the Matrix field with the desired type parameter as S
        /// <example>Matrix = TransitionMatrix.GenerateMatrix(S);</example>
        /// </summary>
        public abstract void InitializeMatrix();
    }
}
