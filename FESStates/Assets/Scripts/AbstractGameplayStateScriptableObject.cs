using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameplayStateScriptableObject : ScriptableObject
{
    public AbstractGameplayStateScriptableObject Parent;
    
    public abstract AbstractGameplayState GenerateState(StateActor actor);

    public bool IsDescendantOf(AbstractGameplayStateScriptableObject other)
    {
        AbstractGameplayStateScriptableObject parent = Parent;
        while (parent is not null)
        {
            if (parent == other) return true;
            parent = parent.Parent;
        }

        return false;
    }

    public bool IsRelatedTo(AbstractGameplayStateScriptableObject other)
    {
        if (other == this) return true;
        
        AbstractGameplayStateScriptableObject parent = Parent;
        while (parent is not null)
        {
            if (parent == other) return true;
            if (other.IsDescendantOf(parent)) return true;
            parent = parent.Parent;
        }

        return false;
    }
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

    public virtual StateComparisonData Compare(AbstractGameplayState other)
    {
        return default;
    }
}
