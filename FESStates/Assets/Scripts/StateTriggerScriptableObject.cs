using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FESState/Trigger/State Trigger")]
public class StateTriggerScriptableObject : AbstractStateTriggerScriptableObject
{

    public bool ForceOverrideModerator;
    public StateModeratorScriptableObject OverrideModerator;
    public List<StatePriorityTag> TryPreserveStates;

    [Space] 
    
    public bool FoceOverrideStates;
    public SerializedDictionary<StatePriorityTag, AbstractGameplayStateScriptableObject> OverrideStates;
    
    [Space]
        
    public bool ReEnterSameStates;

    public override bool Activate(StateActor actor)
    {
        // We want to change the actor's state moderator
        if (OverrideModerator)
        {
            if (!ForceOverrideModerator && OverrideModerator.ModeratorPriority < actor.Moderator.BaseModerator.ModeratorPriority) return false;
            
            StateModerator.MetaStateModerator overrideModerator = StateModerator.GenerateMeta(OverrideModerator);
            Dictionary<StatePriorityTag, AbstractGameplayState> preserveStates = new Dictionary<StatePriorityTag, AbstractGameplayState>();
            if (TryPreserveStates.Count > 0)
            {
                // We want to preserve state(s) at some priorities
                foreach (StatePriorityTag priorityTag in TryPreserveStates)
                {
                    // Ensure the override moderator defines the current state
                    if (actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && !overrideModerator.DefinesState(priorityTag, state.GameplayState)) continue;
                    preserveStates[priorityTag] = state;
                }
            }

            foreach (StatePriorityTag priorityTag in preserveStates.Keys) overrideModerator.ChangeState(priorityTag, preserveStates[priorityTag].GameplayState);
                
            foreach (StatePriorityTag priorityTag in OverrideStates.Keys)
            {
                if (!overrideModerator.DefinesState(priorityTag, OverrideStates[priorityTag]) && !FoceOverrideStates) continue;
                overrideModerator.ChangeState(priorityTag, OverrideStates[priorityTag]);
            }
                
            overrideModerator.FillEmptyStates(actor.Moderator);
                
            actor.Moderator.ImplementMeta(overrideModerator, ReEnterSameStates);
            return true;
        }
        
        if (OverrideStates is not null)
        {
            Dictionary<StatePriorityTag, AbstractGameplayState> activeStates = actor.Moderator.GetActiveStates();
            if (ReEnterSameStates)
            {
                foreach (StatePriorityTag priorityTag in activeStates.Keys)
                {
                    if (OverrideStates.Keys.Contains(priorityTag)) continue;
                    actor.Moderator.InterruptChangeState(priorityTag, activeStates[priorityTag].GameplayState);
                }
            }
            
            foreach (StatePriorityTag priorityTag in OverrideStates.Keys)
            {
                if (!actor.Moderator.DefinesState(priorityTag, OverrideStates[priorityTag]) && !FoceOverrideStates) continue;
                if (OverrideStates[priorityTag] == activeStates[priorityTag].GameplayState && !ReEnterSameStates) continue; 
                actor.Moderator.InterruptChangeState(priorityTag, OverrideStates[priorityTag], false);
            }

            return true;
        }
        
        if (ReEnterSameStates)
        {
            Dictionary<StatePriorityTag, AbstractGameplayState> activeStates = actor.Moderator.GetActiveStates();
            foreach (StatePriorityTag priorityTag in activeStates.Keys) actor.Moderator.InterruptChangeState(priorityTag, activeStates[priorityTag].GameplayState);
            return true;
        }

        return false;
    }
    
    protected virtual void OnValidate()
    {
        if (!OverrideModerator && OverrideStates.Count == 0 && !ReEnterSameStates) throw new Exception($"({name}) State Trigger must define either (or both) Override Moderator or Transition State, or ReEnterSameStates must be true (performs simple moderator reset)");
    }
}
