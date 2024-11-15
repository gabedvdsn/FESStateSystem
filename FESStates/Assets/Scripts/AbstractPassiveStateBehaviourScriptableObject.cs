using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPassiveStateBehaviourScriptableObject : ScriptableObject
{
    public abstract AbstractPassiveStateBehaviour Generate(StateActor actor);
}

public abstract class AbstractPassiveStateBehaviour
{
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnPhysicsUpdate();
    public abstract void OnLeave();
} 