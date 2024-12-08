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
        public SerializedDictionary<AbstractGameplayStateScriptableObject, List<StateTransitionScriptableObject>> Transitions;

        public StateTransitionMatrix<S> GenerateMatrix<S>(S source)
        {
            Dictionary<AbstractGameplayStateScriptableObject, List<StateTransition<S>>> transitions = new Dictionary<AbstractGameplayStateScriptableObject, List<StateTransition<S>>>();
            foreach (AbstractGameplayStateScriptableObject state in Transitions.Keys)
            {
                transitions[state] = Transitions[state].Where(transition => transition != null).Select(transition => new StateTransition<S>(source, transition)).ToList();
            }

            return new StateTransitionMatrix<S>(transitions);
        }

        private void OnValidate()
        {
            if (Transitions is null || Transitions.Count == 0) return;
            
            // Prevent transitions with duplicate predicates from the same state
            foreach (AbstractGameplayStateScriptableObject state in Transitions.Keys)
            {
                HashSet<AbstractTransitionPredicateScriptableObject> seen = new HashSet<AbstractTransitionPredicateScriptableObject>();
                List<int> duplicatePredicates = new List<int>();

                for (int i = 0; i < Transitions[state].Count; i++)
                {
                    if (Transitions[state][i] is null) continue;
                    if (!seen.Add(Transitions[state][i].Predicate))
                    {
                        duplicatePredicates.Add(i);
                        Debug.Log($"Found duplicate predicate for {Transitions[state][i].name}");
                    }
                }

                foreach (int duplicate in duplicatePredicates)
                {
                    Transitions[state][duplicate] = null;
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
        private Dictionary<AbstractGameplayStateScriptableObject, List<StateTransition<S>>> matrix;

        public StateTransitionMatrix(Dictionary<AbstractGameplayStateScriptableObject, List<StateTransition<S>>> matrix)
        {
            this.matrix = matrix;
        }


        public TransitionEvaluationData EvaluateTransitionsFor(AbstractGameplayStateScriptableObject activeState)
        {
            if (!matrix.ContainsKey(activeState)) return new TransitionEvaluationData(false, null);
            List<AbstractGameplayStateScriptableObject> successTargets = new List<AbstractGameplayStateScriptableObject>();
            foreach (StateTransition<S> transition in matrix[activeState]) if (transition.EvaluatePredicate()) successTargets.Add(transition.BaseTransition.To);
            return new TransitionEvaluationData(successTargets.Count > 0, successTargets);
        }

        public bool TryGetTransitionTo(AbstractGameplayStateScriptableObject fromState, AbstractGameplayStateScriptableObject toState, out StateTransition<S> transition)
        {
            transition = null;
            if (!matrix.TryGetValue(fromState, out List<StateTransition<S>> transitions)) return false;
            foreach (StateTransition<S> mTransition in transitions.Where(mTransition => mTransition.BaseTransition.To == toState))
            {
                transition = mTransition;
                return true;
            }

            return false;
        }
        
        public bool SubscribeEventToEvaluate(AbstractGameplayStateScriptableObject fromState, AbstractGameplayStateScriptableObject toState, ref StateTransition<S>.EvaluateAction evaluateAction)
        {
            if (!TryGetTransitionTo(fromState, toState, out StateTransition<S> transition)) return false;
            transition.SubscribeEvaluateEvent(ref evaluateAction);
            return true;
        }

        public void LogMatrix()
        {
            Debug.Log($"[ STATE TRANSITION MATRIX ]");
            foreach (AbstractGameplayStateScriptableObject state in matrix.Keys)
            {
                Debug.Log($"\t[ FROM ] {state.name}");
                foreach (StateTransition<S> transition in matrix[state])
                {
                    Debug.Log($"\t\t[ TO ] {transition.BaseTransition.To.name} [ ON ] {transition.BaseTransition.Predicate.name}");
                }
            }
        }
    }

    public struct TransitionEvaluationData
    {
        public bool Status;
        public List<AbstractGameplayStateScriptableObject> SuccessfulTransitions;

        public TransitionEvaluationData(bool status, List<AbstractGameplayStateScriptableObject> successfulTransitions)
        {
            Status = status;
            SuccessfulTransitions = successfulTransitions;
        }

        public AbstractGameplayStateScriptableObject First() => Status ? SuccessfulTransitions[0] : null;
        public AbstractGameplayStateScriptableObject Any() => Status ? SuccessfulTransitions.RandomChoice() : null;
        public AbstractGameplayStateScriptableObject Last() => Status ? SuccessfulTransitions.Last() : null;
    }

}