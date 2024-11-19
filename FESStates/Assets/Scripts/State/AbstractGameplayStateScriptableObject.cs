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
    public AbstractGameplayStateScriptableObject StateData;
    
    protected AbstractGameplayState(AbstractGameplayStateScriptableObject stateData)
    {
        StateData = stateData;
    }

    /// <summary>
    /// Called when the state is entered
    /// </summary>
    public abstract void Enter();
    
    /// <summary>
    /// Called every frame from the Update method
    /// </summary>
    public abstract void LogicUpdate();
    
    /// <summary>
    /// Called every frame from the LateUpdate method
    /// </summary>
    public abstract void PhysicsUpdate();
    
    /// <summary>
    /// Called when the state is exited from a trigger, as opposed to concluding itself.
    /// </summary>
    public abstract void Interrupt();
    
    /// <summary>
    /// Defines the natural conclusion of this state. Conclude must implement any further state transition. The state should always conclude itself.
    /// </summary>
    public abstract void Conclude();
    
    /// <summary>
    /// Called when the state is exited. Any state-specific exit behaviour should be implemented in Interrupt() and Conclude()
    /// </summary>
    public abstract void Exit();
}
