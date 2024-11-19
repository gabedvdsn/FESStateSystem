using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RunStateConditionalTrigger : AbstractTriggerRunner
{
    public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
    public AbstractStateConditionalTriggerScriptableObject ConditionalTrigger;
    public UnityEvent OnTrueEvent;
    public UnityEvent OnFalseEvent;

    public override void RunDefault()
    {
        StateActor actor = ActorRetrieval.RetrieveActor<StateActor>();
        GameplayStateManager.Instance.RunDefaultConditionalTrigger(actor, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    public override void RunDefaultMany(int count = -1)
    {
        List<StateActor> actors = ActorRetrieval.RetrieveManyActors<StateActor>(count);
        GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }
    public override void RunDefaultAll()
    {
        List<StateActor> actors = ActorRetrieval.RetrieveAllActors<StateActor>();
        GameplayStateManager.Instance.RunDefaultManyConditionalTrigger(actors, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
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
        GameplayStateManager.Instance.RunActorSpecificConditionalAllTrigger<T>(ActorRetrieval, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }

    public void LogFeedback(string message) => Debug.Log(message);
}
