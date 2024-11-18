using UnityEngine;

public abstract class AbstractStateTriggerScriptableObject : ScriptableObject
{
    public abstract bool Activate(StateActor actor);
}
