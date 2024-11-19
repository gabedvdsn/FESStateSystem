using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger")]
public class ConditionalStateTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject, IStateConditional
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

    public override bool StateSpecificActivate(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state)
    {
        if (!LookForStates.ContainsKey(priorityTag)) return false;
        
        if (AllowRelations)
        {
            if (LookForStates[priorityTag].Any(lookForState => !state.IsRelatedTo(lookForState))) return false;
        }
        else if (AllowDescendants)
        {
            if (LookForStates[priorityTag].Any(lookForState => !state.IsDescendantOf(lookForState))) return false;
        }
        else if (!LookForStates[priorityTag].Contains(state)) return false;

        return true;
    }

    public override bool ModeratorSpecificActivate(StateModeratorScriptableObject moderator)
    {
        return LookForModerators.Count <= 0 || LookForModerators.Contains(moderator);
    }
}

public interface IStateConditional
{
    public bool Activate(StateActor actor);
}