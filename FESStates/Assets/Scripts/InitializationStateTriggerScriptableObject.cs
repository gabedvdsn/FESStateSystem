using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Initialization Trigger")]
public class InitializationStateTriggerScriptableObject : AbstractStateTriggerScriptableObject
{
    public StateModeratorScriptableObject OverrideModerator;
    public SerializedDictionary<StatePriorityTag, AbstractGameplayStateScriptableObject> OverrideStates;
    
    public override bool Activate(StateActor actor)
    {
        actor.Moderator = OverrideModerator.GenerateModerator(actor);
        
        foreach (StatePriorityTag priorityTag in OverrideStates.Keys)
        {
            if (!actor.Moderator.DefinesState(priorityTag, OverrideStates[priorityTag])) continue;
            actor.Moderator.ChangeState(priorityTag, OverrideStates[priorityTag]);
        }

        return true;
    }
    
    protected void OnValidate()
    {
        if (!OverrideModerator) throw new Exception("Initialization State Trigger must define Override Moderator");
    }
}
