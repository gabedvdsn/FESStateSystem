using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/State Trigger Group")]
public class StateGroupTriggerScriptableObject : AbstractStateTriggerScriptableObject
{
    public List<StateTriggerScriptableObject> Triggers;
    
    public override bool Activate(StateActor actor, bool flag)
    {
        return Triggers.All(t => t.Activate(actor, flag));
    }
}
