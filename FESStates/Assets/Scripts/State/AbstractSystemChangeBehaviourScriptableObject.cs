using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbstractSystemChangeBehaviourScriptableObject : ScriptableObject
{
    public AbstractStateConditionalTriggerScriptableObject FromConditional;
    public AbstractStateConditionalTriggerScriptableObject ToConditional;
    
    [Space]
    
    public bool SkipChangeToSame = true;

    public void OnModeratorChanged(StateActor actor, StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
    {
        if (oldModerator == newModerator && SkipChangeToSame) return;
        
        if (FromConditional)
        {
            if (!FromConditional.Activate(actor)) return;
        }

        if (ToConditional)
        {
            if (!ToConditional.ModeratorSpecificActivate(newModerator)) return;
        }
        
        OnModeratorChangedBehaviour(actor, oldModerator, newModerator);
    }
    
    public void OnStateChanged(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayState oldState, AbstractGameplayState newState)
    {
        if (oldState.StateData == newState.StateData && SkipChangeToSame) return;
        
        if (FromConditional)
        {
            if (!FromConditional.Activate(actor)) return;
        }

        if (ToConditional)
        {
            if (!ToConditional.StateSpecificActivate(priorityTag, newState.StateData)) return;
        }
        
        OnStateChangedBehaviour(actor, priorityTag, oldState, newState);
        
    }

    protected abstract void OnModeratorChangedBehaviour(StateActor actor, StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator);
    
    protected abstract void OnStateChangedBehaviour(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayState oldState, AbstractGameplayState newState);
}
