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

    public override bool StateSpecificActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state)
    {
        if (!LookForStates.ContainsKey(priorityTag)) return false;
        
        bool status = true;
        if (AllowRelations)
        {
            // If any of the look for states are not related to state
            if (LookForStates[priorityTag].Any(lookForState => !state.IsRelatedTo(lookForState))) status = false;
        }
        else if (AllowDescendants)
        {
            // If any of the look for states are not descended from state
            if (LookForStates[priorityTag].Any(lookForState => !state.IsDescendantOf(lookForState))) status = false;
        }
        else if (!LookForStates[priorityTag].Contains(state)) status = false;

        // Check within actor for the look for states
        if (!status)
        {
            if (LookForStates[priorityTag].Any(s => !actor.Moderator.TryGetStoredState(priorityTag, s, out AbstractGameplayState _))) return false;
        }

        return status;
    }

    public override Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates() => LookForStates;

    public override bool ModeratorSpecificActivate(StateModeratorScriptableObject moderator)
    {
        return LookForModerators.Count <= 0 || LookForModerators.Contains(moderator);
    }
}