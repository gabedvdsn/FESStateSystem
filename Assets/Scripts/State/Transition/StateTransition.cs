using System.Collections.Generic;
using System.Linq;

namespace FESStateSystem
{
    public class StateTransition<S>
    {
        public delegate bool EvaluateAction();

        public StateTransitionScriptableObject BaseTransition;

        protected List<AbstractTransitionPredicate<S>> Predicates;

        protected S Source;

        public StateTransition(S source, StateTransitionScriptableObject transition)
        {
            BaseTransition = transition;
            Predicates = BaseTransition.Predicate.Generate<S>();
            Source = source;
        }

        public bool EvaluatePredicate()
        {
            return Predicates.All(p => p.Evaluate(Source));
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

    public class PlayerStateTransition : StateTransition<DemoPlayerController>
    {

        public PlayerStateTransition(DemoPlayerController source, StateTransitionScriptableObject transition) : base(source, transition)
        {
        }

        protected override bool TriggerEvaluate()
        {
            return true;
        }
    }
}
