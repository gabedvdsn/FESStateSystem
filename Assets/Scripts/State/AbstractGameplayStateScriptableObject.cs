using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameplayStateScriptableObject : ScriptableObject
{
    public AbstractGameplayStateScriptableObject Parent;
    public StateTriggerScriptableObject ConclusionTrigger;
    
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
    public StateActor State;
    
    protected AbstractGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor)
    {
        StateData = stateData;
        State = actor;
    }

    /// <summary>
    ///  Called once when the state is created by the moderator, or when a new moderator is implemented.
    /// </summary>
    public abstract void Initialize();

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
    /// Defines the natural conclusion of this state. The state should always conclude itself. State transition is handled in the base implementation.
    /// </summary>
    public virtual void Conclude()
    {
        if (StateData.ConclusionTrigger) StateData.ConclusionTrigger.Activate(State, false);
        else State.Moderator.ReturnToInitial(StateData);
    }
    
    /// <summary>
    /// Called when the state is exited. Any state-specific exit behaviour should be implemented in Interrupt() and Conclude()
    /// </summary>
    public abstract void Exit();
}
