using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public class StateActor : MonoBehaviour
    {
        public StateIdentifierTagScriptableObject GeneralIdentifier;
    
        public StateModerator Moderator;

        public delegate void DisableAction();

        private DisableAction OnDisableEvent;

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

        protected virtual void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            GameplayStateManager.Instance.UnsubscribeActor(this);
        }

    }
}