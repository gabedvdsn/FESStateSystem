using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActor : MonoBehaviour
{
    public GameplayStateTagScriptableObject GeneralIdentifier;
    
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

    protected virtual void OnDestroy()
    {
        GameplayStateManager.Instance.UnsubscribeActor(this);
    }

}