using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FESStateSystem
{
    public class StateTransition<S> where S : MonoBehaviour
    {
        public readonly StateTransitionScriptableObject BaseTransition;

        private readonly List<AbstractTransitionPredicate<S>> Predicates;
        
        private StateTransition(StateTransitionScriptableObject transitionData)
        {
            BaseTransition = transitionData;
            Predicates = BaseTransition.Predicate.Generate<S>();
        }

        public bool EvaluatePredicate(S source, StateActor actor)
        {
            return Predicates.All(p => p.Evaluate(source, actor));
        }

        public static StateTransition<S> Generate(StateTransitionScriptableObject transitionData)
        {
            return new StateTransition<S>(transitionData);
        }
    }
}
