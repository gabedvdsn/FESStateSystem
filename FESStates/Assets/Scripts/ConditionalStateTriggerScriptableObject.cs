using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger")]
public class ConditionalStateTriggerScriptableObject : AbstractStateTriggerScriptableObject, IStateConditional
{
    public List<StateModeratorScriptableObject> LookForModerators;
    
    [Space]
    
    public SerializedDictionary<StatePriorityTag, List<AbstractGameplayStateScriptableObject>> LookForStates;

    public bool AllowDescendants;
    public bool AllowRelations;
    
    public override bool Activate(StateActor actor)
    {
        if (LookForModerators.Count > 0 && !LookForModerators.Contains(actor.Moderator.BaseModerator)) return false;
        
        foreach (StatePriorityTag priorityTag in LookForStates.Keys.Where(priorityTag => LookForStates[priorityTag].Count != 0))
        {
            if (!actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state)) return false;
            if (AllowRelations)
            {
                if (LookForStates[priorityTag].Any(lookForState => !state.GameplayState.IsRelatedTo(lookForState))) return false;
            }
            else if (AllowDescendants)
            {
                if (LookForStates[priorityTag].Any(lookForState => !state.GameplayState.IsDescendantOf(lookForState))) return false;
            }
            else if (!LookForStates[priorityTag].Contains(state.GameplayState)) return false;
        }

        return true;
    }
}

public interface IStateConditional
{
    public bool Activate(StateActor actor);
}