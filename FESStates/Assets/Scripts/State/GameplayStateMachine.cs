using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayStateMachine
{
    public AbstractGameplayState CurrentState;

    public void Initialize(AbstractGameplayState initialState)
    {
        CurrentState = initialState;
        initialState.Enter();
    }

    public void ChangeState(AbstractGameplayState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void InterruptChangeState(AbstractGameplayState newState)
    {
        CurrentState.Interrupt();
        ChangeState(newState);
    }
}
