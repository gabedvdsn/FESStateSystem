using UnityEngine;

namespace FESStateSystem
{
    /// <summary>
    /// Transition predicates are utilized to regulate the behaviour of your state actor
    /// </summary>
    public abstract class AbstractTransitionPredicateScriptableObject : ScriptableObject
    {
        public abstract AbstractTransitionPredicate<S> GeneratePredicate<S>(S source);
    }
}