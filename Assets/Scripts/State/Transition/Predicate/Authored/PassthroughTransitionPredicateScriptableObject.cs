using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Transition/Predicate/Passthrough")]
    public class PassthroughTransitionPredicateScriptableObject : AbstractTransitionPredicateScriptableObject
    {
        public override List<AbstractTransitionPredicate<S>> Generate<S>()
        {
            return new List<AbstractTransitionPredicate<S>>()
            {
                new PassthroughTransitionPredicate<S>()
            };
        }

        public class PassthroughTransitionPredicate<S> : AbstractTransitionPredicate<S>
        {
            
            public override bool Evaluate(S source)
            {
                return true;
            }
        }
    }
}
