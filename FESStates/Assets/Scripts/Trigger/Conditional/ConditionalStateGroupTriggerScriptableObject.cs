using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Group Trigger")]
public class ConditionalStateGroupTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject
{
    public List<AbstractStateConditionalTriggerScriptableObject> Conditionals;
    
    public override bool Activate(StateActor actor)
    {
        return Conditionals.All(_ => Conditionals.Any(c => c.Activate(actor)));
    }

    public override bool StateSpecificActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state)
    {
        // return Conditionals.All(c => c.StateSpecificActivate(actor, priorityTag, state));
        return Conditionals.All(_ => Conditionals.Any(c => c.StateSpecificActivate(actor, priorityTag, state)));
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

    public override bool ModeratorSpecificActivate(StateModeratorScriptableObject moderator)
    {
        return Conditionals.All(c => c.ModeratorSpecificActivate(moderator));
    }
}
