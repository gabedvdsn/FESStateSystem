using UnityEngine;
using FESStateSystem;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "FESState/Authored/State/Transition Predicate/IsGrounded")]
public class IsGroundedTransitionPredicateScriptableObject : AbstractTransitionPredicateScriptableObject
{
    public override List<AbstractTransitionPredicate<S>> Generate<S>()
    {
        return new List<AbstractTransitionPredicate<S>>()
        {
            new IsGroundedTransitionPredicate<S>()
        };
    }

    public class IsGroundedTransitionPredicate<S> : AbstractTransitionPredicate<S>
    {
        public override bool Evaluate(S source, StateActor actor)
        {
            throw new System.NotImplementedException();
        }
    }
}

