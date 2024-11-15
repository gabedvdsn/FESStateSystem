using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameplayStateScriptableObject : ScriptableObject
{
    public List<AbstractPassiveStateBehaviourScriptableObject> PassiveStateBehaviours;
    
    public abstract AbstractGameplayState GenerateState(StateActor actor);
}

public abstract class AbstractGameplayState
{
    public AbstractGameplayStateScriptableObject GameplayState;
    
    protected AbstractGameplayState(AbstractGameplayStateScriptableObject gameplayState)
    {
        GameplayState = gameplayState;
    }

    /// <summary>
    /// Must only be called by State Machine
    /// </summary>
    public virtual void Enter()
    {
        Debug.Log($"Enter {GameplayState.name}");
    }
    public virtual void LogicUpdate()
    {
        // Debug.Log($"LogicUpdate {GameplayState.name}");
    }
    public virtual void PhysicsUpdate()
    {
        // Debug.Log($"PhysicsUpdate {GameplayState.name}");
    }
    public virtual void Interrupt()
    {
        Debug.Log($"Interrupt {GameplayState.name}");
    }
    public virtual void Conclude()
    {
        Debug.Log($"Conclude {GameplayState.name}");
    }
    
    /// <summary>
    /// Must only be called by State Machine
    /// </summary>
    public virtual void Exit()
    {
        Debug.Log($"Exit {GameplayState.name}");
    }
}
