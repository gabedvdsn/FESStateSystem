using System;
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
        public PostConditionalEvent OnTrueEvent;
        public PostConditionalEvent OnFalseEvent;

        public override void Run(bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveActor(out StateActor actor))
            {
                if (ConditionalTrigger.FalseOnNullActor) OnFalseEvent?.Run(actor, flag);
                return;
            }
            GameplayStateManager.Instance.RunDefaultConditionalTrigger(actor, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void Run(StateActor actor, bool flag = true)
        {
            GameplayStateManager.Instance.RunDefaultConditionalTrigger(actor, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunMany(int count = -1, bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveManyActors(count, out List<StateActor> actors))
            {
                if (ConditionalTrigger.FalseOnNullActor) OnFalseEvent?.RunNull();
                return;
            }
            GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunMany(List<StateActor> actors, bool flag = true)
        {
            GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public override void RunAll(bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveManyActors(-1, out List<StateActor> actors))
            {
                if (ConditionalTrigger.FalseOnNullActor) OnFalseEvent?.RunNull();
                return;
            }
            GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent, flag);
        }
        
        public void LogFeedback(string message) => Debug.Log(message);
    }

    [Serializable]
    public class PostConditionalEvent
    {
        public UnityEvent<StateActor> Event;
        public StateTriggerScriptableObject StateTrigger;

        public void RunNull()
        {
            Event?.Invoke(null);
            if (StateTrigger) StateTrigger.Activate(null, false);
        }

        public void Run(StateActor actor, bool flag)
        {
            Event?.Invoke(actor);
            if (StateTrigger) StateTrigger.Activate(actor, flag);
        }
        
        public void Run(List<StateActor> actors, bool flag)
        {
            foreach (StateActor actor in actors)
            {
                Event?.Invoke(actor);
                if (StateTrigger) StateTrigger.Activate(actor, flag);
            }
        }
    }
}