using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UICheckConditionalTrigger : MonoBehaviour
{
    public StateActor Actor;
    public ConditionalStateTriggerScriptableObject ConditionalTrigger;
    public UnityEvent OnTrueEvent;
    public UnityEvent OnFalseEvent;

    public void OnClick()
    {
        if (ConditionalTrigger.Activate(Actor)) OnTrueEvent?.Invoke();
        else OnFalseEvent?.Invoke();
    }

    public void Feedback(string message) => Debug.Log(message);
}
