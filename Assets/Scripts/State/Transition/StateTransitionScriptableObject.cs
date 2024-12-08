using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Transition/Transition")]
    public class StateTransitionScriptableObject : ScriptableObject
    {
        public AbstractTransitionPredicateScriptableObject Predicate;
        public AbstractGameplayStateScriptableObject To;
    }
}