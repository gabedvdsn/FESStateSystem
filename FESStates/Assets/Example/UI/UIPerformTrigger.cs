using UnityEngine;

public class UIPerformTrigger : MonoBehaviour
{
    public StateActor Actor;
    public StateTriggerScriptableObject Trigger;

    public void OnClick()
    {
        Trigger.Activate(Actor);
    }
}
