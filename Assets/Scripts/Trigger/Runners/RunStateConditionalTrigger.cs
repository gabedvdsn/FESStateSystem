using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    public class RunStateConditionalTrigger : AbstractTriggerRunner
    {
        public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
        public AbstractStateConditionalTriggerScriptableObject ConditionalTrigger;
        public UnityEvent<StateActor> OnTrueEvent;
        public UnityEvent<StateActor> OnFalseEvent;

        public override void RunDefault(bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveActor(out StateActor actor))
            {
                if (ConditionalTrigger.FalseOnNullActor) OnFalseEvent?.Invoke(actor);
                return;
            }
            GameplayStateManager.Instance.RunDefaultConditionalTrigger(actor, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunDefault(StateActor actor, bool flag = true)
        {
            GameplayStateManager.Instance.RunDefaultConditionalTrigger(actor, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunDefaultMany(int count = -1, bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveManyActors(count, out List<StateActor> actors))
            {
                if (ConditionalTrigger.FalseOnNullActor) OnFalseEvent?.Invoke(null);
                return;
            }
            GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunDefaultMany(List<StateActor> actors, bool flag = true)
        {
            GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunDefaultAll(bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveManyActors(-1, out List<StateActor> actors))
            {
                if (ConditionalTrigger.FalseOnNullActor) OnFalseEvent?.Invoke(null);
                return;
            }
            GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public void LogFeedback(string message) => Debug.Log(message);
    }
}