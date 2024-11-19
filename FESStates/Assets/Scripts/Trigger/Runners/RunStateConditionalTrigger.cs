using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RunStateConditionalTrigger : MonoBehaviour
{
    public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
    public AbstractStateConditionalTriggerScriptableObject ConditionalTrigger;
    public UnityEvent OnTrueEvent;
    public UnityEvent OnFalseEvent;

    public void RunDefault()
    {
        StateActor actor = ActorRetrieval.RetrieveActor<StateActor>();
        GameplayStateManager.Instance.RunStateConditionalTrigger(actor, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }

    public void RunTypeSpecific<T>() where T : StateActor
    {
        GameplayStateManager.Instance.RunActorSpecificConditionalTrigger<T>(ActorRetrieval, ConditionalTrigger, OnTrueEvent, OnFalseEvent);
    }

    public void LogFeedback(string message) => Debug.Log(message);
}
