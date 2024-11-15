using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActor : MonoBehaviour
{
    public GameplayStateTagScriptableObject GeneralIdentifier;
    
    [Space]
    
    public List<AbstractPassiveStateBehaviourScriptableObject> PassiveStateBehaviours;
    
    public StateModerator Moderator;

    protected virtual void Awake()
    {
        GameplayStateManager.Instance.SubscribeActor(this);
    }
    
    protected virtual void Update()
    {
        Moderator?.RunStatesLogicUpdate();
    }
    
    protected virtual void LateUpdate()
    {
        Moderator?.RunStatesPhysicsUpdate();
    }

    public void SetModerator(StateModerator.MetaStateModerator metaModerator)
    {
        
    }
    
}