using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateConditionalTriggerScriptableObject : AbstractStateTriggerScriptableObject
{
    public bool FalseOnNullActor = true;
    
    public abstract bool StateSpecificActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state);

    public abstract Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates();

    public abstract bool ModeratorSpecificActivate(StateModeratorScriptableObject moderator);
}
