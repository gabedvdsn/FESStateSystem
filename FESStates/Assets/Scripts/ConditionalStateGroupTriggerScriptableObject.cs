using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Group Trigger")]
public class ConditionalStateGroupTriggerScriptableObject : AbstractStateTriggerScriptableObject, IStateConditional
{
    public List<ConditionalStateTriggerScriptableObject> Conditionals;
    
    public override bool Activate(StateActor actor)
    {
        return Conditionals.All(c => c.Activate(actor));
    }
}
