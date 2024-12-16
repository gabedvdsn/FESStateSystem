using UnityEngine;
using FESStateSystem;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "FESState/Authored/State/Transition Predicate/HasStamina")]
public class HasStaminaTransitionPredicateScriptableObject : AbstractTransitionPredicateScriptableObject
{
    public override List<AbstractTransitionPredicate<S>> Generate<S>()
    {
        return new List<AbstractTransitionPredicate<S>>()
        {
            new HasStaminaTransitionPredicate<S>()
        };
    }

    public class HasStaminaTransitionPredicate<S> : AbstractTransitionPredicate<S>
    {
        public override bool Evaluate(S source, StateActor actor)
        {
            throw new System.NotImplementedException();
        }
    }
}

