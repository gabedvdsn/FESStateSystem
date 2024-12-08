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

        private void Awake()
        {
            GameplayStateManager.Instance.SubscribeActor(this);
            OnAwake();
        }

        protected virtual void OnAwake() { }
    
        private void Update()
        {
            Moderator?.RunStatesLogicUpdate();
            OnUpdate();
        }

        protected virtual void OnUpdate() { }
    
        private void FixedUpdate()
        {
            Moderator?.RunStatesPhysicsUpdate();
            OnFixedUpdate();
        }
        
        protected virtual void OnFixedUpdate() { }

        private void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            GameplayStateManager.Instance.UnsubscribeActor(this);
        }

    }
}