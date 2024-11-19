using UnityEngine;

public class RunStateTrigger : MonoBehaviour
{
    public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
    public StateTriggerScriptableObject Trigger;

    public void RunDefault()
    {
        StateActor actor = ActorRetrieval.RetrieveActor<StateActor>();
        if (actor is null) return;
        
        GameplayStateManager.Instance.RunStateTrigger(actor, Trigger);
    }

    public void RunActorSpecific<T>() where T : StateActor
    {
        GameplayStateManager.Instance.RunActorSpecificTrigger<T>(ActorRetrieval, Trigger);
    }
}
