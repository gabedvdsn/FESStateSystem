using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger Group")]
public class ConditionalStateGroupTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject
{
    public List<AbstractStateConditionalTriggerScriptableObject> Conditionals;
    
    public override bool Activate(StateActor actor, bool flag)
    {
        return Conditionals.All(c => c.Activate(actor, flag));
    }

    public override bool PreStateChangeActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState)
    {
        // The new newState should align with one of the conditionals
        // The other conditionals should hold true for active/stored states (depending on conditional)
        return Conditionals.All(c => c.PreStateChangeActivate(actor, priorityTag, newState));
    }

    public override Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates()
    {
        Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> states =
            new Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>>();
        foreach (AbstractStateConditionalTriggerScriptableObject conditional in Conditionals)
        {
            Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> cStates = conditional.GetStates();
            foreach (StatePriorityTagScriptableObject priorityTag in cStates.Keys)
            {
                if (states.ContainsKey(priorityTag))
                {
                    foreach (AbstractGameplayStateScriptableObject state in cStates[priorityTag].Where(state => !states[priorityTag].Contains(state)))
                    {
                        states[priorityTag].Add(state);
                    }
                }
                else states[priorityTag] = cStates[priorityTag];
            }
        }

        return states;
    }

    public override bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator)
    {
        return Conditionals.All(c => c.PreModeratorChangeActivate(moderator));
    }
}
