using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/Trigger/State Trigger Group", fileName = "StateTriggerGroup")]
    public class StateTriggerGroupScriptableObject : AbstractStateTriggerScriptableObject
    {
        public List<StateTriggerScriptableObject> Triggers;
    
        public override bool Activate(StateActor actor, bool flag)
        {
            return Triggers.All(t => t.Activate(actor, flag));
        }
    }
}