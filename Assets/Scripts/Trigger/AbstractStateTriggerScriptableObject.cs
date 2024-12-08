using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractStateTriggerScriptableObject : ScriptableObject
    {
        public abstract bool Activate(StateActor actor, bool flag);
    }
}