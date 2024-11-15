using System;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Initialization Trigger")]
public class InitializationStateTriggerScriptableObject : StateTriggerScriptableObject
{
    public override void Activate(StateActor actor)
    {
        actor.Moderator = OverrideModerator.GenerateModerator(actor);
        
        foreach (StatePriorityTag priorityTag in OverrideStates.Keys)
        {
            if (!actor.Moderator.DefinesState(priorityTag, OverrideStates[priorityTag])) continue;
            actor.Moderator.ChangeState(priorityTag, OverrideStates[priorityTag]);
        }
    }
    
    protected override void OnValidate()
    {
        if (!OverrideModerator) throw new Exception("Initialization State Trigger must define Override Moderator");
    }
}
