using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateConditionalTriggerScriptableObject : AbstractStateTriggerScriptableObject
{
    public bool FalseOnNullActor = true;
    
    public abstract bool StateSpecificActivate(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state);

    public abstract bool ModeratorSpecificActivate(StateModeratorScriptableObject moderator);
}
