using System.Collections.Generic;

namespace FESStateSystem
{
    public class StateTransition<S>
    {
        public delegate void EvaluateAction();

        public StateTransitionScriptableObject BaseTransition;

        private AbstractTransitionPredicate<S> Predicate;

        public StateTransition(S source, StateTransitionScriptableObject transition)
        {
            BaseTransition = transition;
            Predicate = BaseTransition.Predicate.GeneratePredicate(source);
        }

        public bool EvaluatePredicate()
        {
            return Predicate.Evaluate();
        }

        public void SubscribeEvaluateEvent(ref EvaluateAction evaluateDelegate)
        {
            evaluateDelegate += TriggerEvaluate;
        }

        public void UnsubscribeEvaluateEvent(ref EvaluateAction evaluateDelegate)
        {
            evaluateDelegate -= TriggerEvaluate;
        }
        
        protected virtual void TriggerEvaluate()
        {
            
        }
    }
}
