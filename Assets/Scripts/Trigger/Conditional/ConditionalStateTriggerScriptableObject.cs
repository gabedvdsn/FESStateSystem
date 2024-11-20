using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger")]
public class ConditionalStateTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject
{
    [Header("Moderator")]
    
    public List<StateModeratorScriptableObject> LookForModerators;
    
    [Header("States")]
    
    public SerializedDictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> LookForStates;

    public bool LookForAny = true;
    public bool ActiveStatesOnly = true;
    public bool AllowDescendants;
    public bool AllowRelations;
    
    public override bool Activate(StateActor actor)
    {
        if (actor.Moderator is null) return false;
        if (LookForModerators.Count > 0 && !LookForModerators.Contains(actor.Moderator.BaseModerator)) return false;
        
        foreach (StatePriorityTagScriptableObject priorityTag in LookForStates.Keys.Where(priorityTag => LookForStates[priorityTag].Count != 0))
        {
            if (!actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state)) return false;
            if (AllowRelations)
            {
                if (LookForStates[priorityTag].Any(lookForState => !state.StateData.IsRelatedTo(lookForState))) return false;
            }
            else if (AllowDescendants)
            {
                if (LookForStates[priorityTag].Any(lookForState => !state.StateData.IsDescendantOf(lookForState))) return false;
            }
            else if (!LookForStates[priorityTag].Contains(state.StateData)) return false;
        }

        return true;
    }

    public override bool PreStateChangeActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState)
    {
        if (!LookForStates.ContainsKey(priorityTag)) return false;
        
        bool status = true;
        if (AllowRelations)
        {
            // If any of the look for states are not related to newState
            if (LookForAny)
            {
                if (LookForStates[priorityTag].Any(lookForState => !newState.IsRelatedTo(lookForState))) status = false;
            }
            else
            {
                if (!LookForStates[priorityTag].All(newState.IsRelatedTo)) status = false;
            }
        }
        else if (AllowDescendants)
        {
            // If any of the look for states are not descended from newState
            if (LookForAny)
            {
                if (LookForStates[priorityTag].Any(lookForState => !newState.IsDescendantOf(lookForState))) status = false;
            }
            else
            {
                if (!LookForStates[priorityTag].All(newState.IsDescendantOf)) status = false;
            }
        }
        else
        {
            if (LookForAny)
            {
                if (!LookForStates[priorityTag].Contains(newState)) status = false;
            }
            else
            {
                if (LookForStates[priorityTag].Any(s => s != newState)) status = false;
            }
        }

        // Check within actor for the look for states
        if (!status)
        {
            if (ActiveStatesOnly)
            {
                if (!LookForStates[priorityTag].All(s => actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && state.StateData == s)) return false;
            }
            
        }

        return status;
    }

    public override Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates() => LookForStates;

    public override bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator)
    {
        return LookForModerators.Count <= 0 || LookForModerators.Contains(moderator);
    }
}