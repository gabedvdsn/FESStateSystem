using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Transition/Predicate/Group", order = 0)]
    public class TransitionPredicateGroupScriptableObject : AbstractTransitionPredicateScriptableObject
    {
        public List<AbstractTransitionPredicateScriptableObject> Predicates;
        public override List<AbstractTransitionPredicate<S>> Generate<S>()
        {
            List<AbstractTransitionPredicate<S>> predicates = new List<AbstractTransitionPredicate<S>>();
            foreach (AbstractTransitionPredicateScriptableObject predicate in Predicates)
            {
                predicates.AddRange(predicate.Generate<S>());
            }

            return predicates;
        }
    }
}
