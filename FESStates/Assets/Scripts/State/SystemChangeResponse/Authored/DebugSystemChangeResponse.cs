using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/System Change/Debug")]
public class DebugSystemChangeResponse : AbstractSystemChangeResponseScriptableObject
{
    protected override void OnModeratorChangedBehaviour(StateActor actor, StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
    {
        Debug.Log($"[{actor.name}] {oldModerator.name} has been changed to {newModerator.name}");
    }
    
    protected override void OnStateChangedBehaviour(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayState oldState, AbstractGameplayState newState)
    {
        Debug.Log($"[{actor.name}] {oldState?.StateData.name} has been changed to {newState.StateData.name} with priority: {priorityTag.name} ({priorityTag.Priority})");
    }
}
