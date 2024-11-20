using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateConditionalTriggerScriptableObject : AbstractStateTriggerScriptableObject
{
    public bool FalseOnNullActor = true;
    
    public abstract bool PreStateChangeActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState);

    public abstract Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates();

    public abstract bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator);
}
