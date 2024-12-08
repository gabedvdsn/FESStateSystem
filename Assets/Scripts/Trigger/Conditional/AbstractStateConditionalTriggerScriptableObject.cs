using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractStateConditionalTriggerScriptableObject : AbstractStateTriggerScriptableObject
    {
        public bool FalseOnNullActor = true;
    
        public abstract bool PreStateChangeActivate(StateActor actor, StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState);

        public abstract Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates();

        public abstract bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator);
    }
}