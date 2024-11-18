using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UICheckConditionalGroupTrigger : MonoBehaviour
{
    public StateActor Actor;
    public ConditionalStateGroupTriggerScriptableObject ConditionalGroupTrigger;
    public UnityEvent OnTrueEvent;
    public UnityEvent OnFalseEvent;

    public void OnClick()
    {
        if (ConditionalGroupTrigger.Activate(Actor)) OnTrueEvent?.Invoke();
        else OnFalseEvent?.Invoke();
    }

    public void Feedback(string message) => Debug.Log(message);
}
