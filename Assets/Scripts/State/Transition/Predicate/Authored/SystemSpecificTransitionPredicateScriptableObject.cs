using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public class SystemSpecificTransitionPredicateScriptableObject : AbstractTransitionPredicateScriptableObject
    {
        public ConditionalStateTriggerScriptableObject Conditional;
        
        public override List<AbstractTransitionPredicate<S>> Generate<S>()
        {
            return new List<AbstractTransitionPredicate<S>>()
            {
                new SystemSpecificTransitionPredicate<S>()
                {
                    Conditional = Conditional
                }
            };
        }

        private class SystemSpecificTransitionPredicate<S> : AbstractTransitionPredicate<S> where S : MonoBehaviour
        {
            public ConditionalStateTriggerScriptableObject Conditional;
            private StateActor Actor;
            
            public override bool Evaluate(S source, StateActor actor)
            {
                return Conditional.Activate(actor, false);
            }
        }
    }
}
