using System.Collections.Generic;
using System.Linq;

namespace FESStateSystem
{
    public class StateTransition<S>
    {
        public delegate bool EvaluateAction();

        public StateTransitionScriptableObject BaseTransition;

        protected List<AbstractTransitionPredicate<S>> Predicates;
        
        public StateTransition(StateTransitionScriptableObject transition)
        {
            BaseTransition = transition;
            Predicates = BaseTransition.Predicate.Generate<S>();
        }

        public bool EvaluatePredicate(S source)
        {
            return Predicates.All(p => p.Evaluate(source));
        }

        public void SubscribeEvaluateEvent(ref EvaluateAction evaluateDelegate)
        {
            evaluateDelegate += TriggerEvaluate;
        }

        public void UnsubscribeEvaluateEvent(ref EvaluateAction evaluateDelegate)
        {
            evaluateDelegate -= TriggerEvaluate;
        }
        
        protected virtual bool TriggerEvaluate()
        {
            return false;
        }
    }
}
