using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RunStateConditionalTrigger : AbstractTriggerRunner
{
    public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
    public AbstractStateConditionalTriggerScriptableObject ConditionalTrigger;
    public UnityEvent<StateActor> OnTrueEvent;
    public UnityEvent<StateActor> OnFalseEvent;

    public override void RunDefault()
    {
        GameplayStateManager.Instance.RunDefaultConditionalTrigger(ActorRetrieval, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    public override void RunDefaultMany(int count = -1)
    {
        GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(ActorRetrieval, count, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    public override void RunDefaultAll()
    {
        GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(ActorRetrieval, -1, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    
    public override void RunActorSpecific<T>()
    {
        GameplayStateManager.Instance.RunActorSpecificConditionalTrigger<T>(ActorRetrieval, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    public override void RunActorSpecificMany<T>(int count = -1)
    {
        GameplayStateManager.Instance.RunActorSpecificConditionalManyTrigger<T>(ActorRetrieval, count, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    public override void RunActorSpecificAll<T>()
    {
        GameplayStateManager.Instance.RunActorSpecificConditionalManyTrigger<T>(ActorRetrieval, -1, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }


    public void LogFeedback(string message) => Debug.Log(message);
    public void LogActorConditional(StateActor actor) => Debug.Log($"[{actor.name}] {ConditionalTrigger.name}");
}
