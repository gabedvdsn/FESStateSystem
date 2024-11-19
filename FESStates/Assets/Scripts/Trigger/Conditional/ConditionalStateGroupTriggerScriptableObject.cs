using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Group Trigger")]
public class ConditionalStateGroupTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject, IStateConditional
{
    public List<AbstractStateConditionalTriggerScriptableObject> Conditionals;
    
    public override bool Activate(StateActor actor)
    {
        return Conditionals.All(c => c.Activate(actor));
    }

    public override bool StateSpecificActivate(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state)
    {
        return Conditionals.All(c => c.StateSpecificActivate(priorityTag, state));
    }

    public override bool ModeratorSpecificActivate(StateModeratorScriptableObject moderator)
    {
        return Conditionals.All(c => c.ModeratorSpecificActivate(moderator));
    }
}
