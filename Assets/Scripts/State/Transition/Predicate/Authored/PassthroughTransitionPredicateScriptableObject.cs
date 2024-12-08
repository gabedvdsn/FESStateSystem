using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Transition/Predicate/Passthrough")]
    public class PassthroughTransitionPredicateScriptableObject : AbstractTransitionPredicateScriptableObject
    {
        public override AbstractTransitionPredicate<S> GeneratePredicate<S>(S source)
        {
            return new PassthroughTransitionPredicate<S>(source);
        }

        public class PassthroughTransitionPredicate<S> : AbstractTransitionPredicate<S>
        {
            public PassthroughTransitionPredicate(S source) : base(source)
            {
            }
            
            public override bool Evaluate()
            {
                return true;
            }
        }
    }
}
