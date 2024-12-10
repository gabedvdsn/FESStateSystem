using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    /// <summary>
    /// Transition predicates are utilized to regulate the behaviour of your state actor
    /// </summary>
    public abstract class AbstractTransitionPredicateScriptableObject : ScriptableObject
    {
        [Tooltip("Allow as predicate for multiple transitions from the same state")]
        public bool AllowForManyTransitions;

        public abstract List<AbstractTransitionPredicate<S>> Generate<S>() where S : MonoBehaviour;
    }
}