using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Transition/Transition Matrix")]
    public class StateTransitionMatrixScriptableObject : ScriptableObject
    {
        [SerializedDictionary("From", "Transitions")]
        public SerializedDictionary<AbstractGameplayStateScriptableObject, StateTransitionData> Transitions;

        public StateTransitionMatrix<S> GenerateMatrix<S>(S source)
        {
            Dictionary<AbstractGameplayStateScriptableObject, LiveStateTransitionData<S>> transitions = new Dictionary<AbstractGameplayStateScriptableObject, LiveStateTransitionData<S>>();
            foreach (AbstractGameplayStateScriptableObject state in Transitions.Keys)
            {
                transitions[state] = new LiveStateTransitionData<S>()
                {
                    Transitions =
                        Transitions[state].TransitionData.Where(transition => transition != null).Select(transition => new StateTransition<S>(transition)).ToList(),
                    Method = Transitions[state].TransitionMethod
                };
            }

            return new StateTransitionMatrix<S>(source, transitions);
        }

        private void OnValidate()
        {
            if (Transitions is null || Transitions.Count == 0) return;
            
            // Prevent transitions with duplicate predicates from the same state
            foreach (AbstractGameplayStateScriptableObject state in Transitions.Keys)
            {
                HashSet<AbstractTransitionPredicateScriptableObject> seen = new HashSet<AbstractTransitionPredicateScriptableObject>();
                List<int> duplicatePredicates = new List<int>();

                for (int i = 0; i < Transitions[state].TransitionData.Count; i++)
                {
                    if (Transitions[state].TransitionData[i] is null) continue;
                    if (!seen.Add(Transitions[state].TransitionData[i].Predicate) && !Transitions[state].TransitionData[i].Predicate.AllowForManyTransitions)
                    {
                        duplicatePredicates.Add(i);
                        Debug.Log($"Found duplicate predicate for {Transitions[state].TransitionData[i].name}");
                    }
                }

                foreach (int duplicate in duplicatePredicates)
                {
                    Transitions[state].TransitionData[duplicate] = null;
                }
            }
        }
    }

    /// <summary>
    /// Represents an edge within a state transition graph.
    /// </summary>
    /// <typeparam name="S">The Source type (e.g. Player, Weather, Camera) which is encapsulated by the predicate implementation.</typeparam>
    public class StateTransitionMatrix<S>
    {
        private Dictionary<AbstractGameplayStateScriptableObject, LiveStateTransitionData<S>> Matrix;
        private S Source;

        public StateTransitionMatrix(S source, Dictionary<AbstractGameplayStateScriptableObject, LiveStateTransitionData<S>> matrix)
        {
            Source = source;
            Matrix = matrix;
        }
        
        public bool TryEvaluateTransitionsFor(AbstractGameplayStateScriptableObject activeState, out TransitionEvaluationResult result)
        {
            if (!Matrix.ContainsKey(activeState))
            {
                result = new TransitionEvaluationResult(false, null, TransitionSelectionMethod.Any);
                return false;
            }

            List<AbstractGameplayStateScriptableObject> successTargets = new List<AbstractGameplayStateScriptableObject>();
            foreach (StateTransition<S> transition in Matrix[activeState].Transitions)
            {
                if (transition.EvaluatePredicate(Source)) successTargets.Add(transition.BaseTransition.To);
            }

            result = new TransitionEvaluationResult(successTargets.Count > 0, successTargets, Matrix[activeState].Method);
            return result.Status;
        }

        public void LogMatrix()
        {
            Debug.Log($"[ STATE TRANSITION MATRIX ] {typeof(S)}");
            foreach (AbstractGameplayStateScriptableObject state in Matrix.Keys)
            {
                Debug.Log($"\t[ FROM ] {state.name} [ BY ] {Matrix[state].Method}");
                foreach (StateTransition<S> transition in Matrix[state].Transitions)
                {
                    Debug.Log($"\t\t[ TO ] {transition.BaseTransition.To.name} [ ON ] {transition.BaseTransition.Predicate.name}");
                }
            }
        }
    }

    [Serializable]
    public class StateTransitionData
    {
        [Tooltip("When multiple predicates evaluate to true, how should the system choose the state to transition to")]
        public TransitionSelectionMethod TransitionMethod = TransitionSelectionMethod.First;
        public List<StateTransitionScriptableObject> TransitionData;
    }

    public class LiveStateTransitionData<S>
    {
        public List<StateTransition<S>> Transitions;
        public TransitionSelectionMethod Method;
    }

    public struct TransitionEvaluationResult
    {
        public bool Status;
        public List<AbstractGameplayStateScriptableObject> SuccessfulTransitions;
        public TransitionSelectionMethod Method;

        public TransitionEvaluationResult(bool status, List<AbstractGameplayStateScriptableObject> successfulTransitions, TransitionSelectionMethod method)
        {
            Status = status;
            SuccessfulTransitions = successfulTransitions;
            Method = method;
        }

        public AbstractGameplayStateScriptableObject First => Status ? SuccessfulTransitions[0] : null;
        public AbstractGameplayStateScriptableObject Any => Status ? SuccessfulTransitions.RandomChoice() : null;
        public AbstractGameplayStateScriptableObject Last => Status ? SuccessfulTransitions.Last() : null;
        public AbstractGameplayStateScriptableObject HighestPriority => Status ? SuccessfulTransitions.OrderByDescending(state => state.SelectionPriority).FirstOrDefault() : null;
        public AbstractGameplayStateScriptableObject LowestPriority => Status ? SuccessfulTransitions.OrderBy(state => state.SelectionPriority).FirstOrDefault() : null;
    }

    public enum TransitionSelectionMethod
    {
        First,
        Any,
        Last,
        HighPriority,
        LowPriority
    }

}