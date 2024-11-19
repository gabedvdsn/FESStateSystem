using System.Collections.Generic;
using UnityEngine;

public class RunStateTrigger : AbstractTriggerRunner
{
    public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
    public StateTriggerScriptableObject Trigger;

    public override void RunDefault()
    {
        StateActor actor = ActorRetrieval.RetrieveActor<StateActor>();
        if (actor is null) return;
        
        GameplayStateManager.Instance.RunDefaultTrigger(actor, Trigger);
    }

    public override void RunDefaultMany(int count = -1)
    {
        List<StateActor> actors = ActorRetrieval.RetrieveManyActors<StateActor>(count);
        if (actors is null) return;
        
        GameplayStateManager.Instance.RunDefaultManyTrigger(actors, Trigger);
    }
    public override void RunDefaultAll()
    {
        List<StateActor> actors = ActorRetrieval.RetrieveAllActors<StateActor>();
        if (actors is null) return;
        
        GameplayStateManager.Instance.RunDefaultManyTrigger(actors, Trigger);
    }

    public override void RunActorSpecific<T>()
    {
        GameplayStateManager.Instance.RunActorSpecificTrigger<T>(ActorRetrieval, Trigger);
    }

    public override void RunActorSpecificMany<T>(int count = -1)
    {
        GameplayStateManager.Instance.RunActorSpecificManyTrigger<T>(ActorRetrieval, count, Trigger);
    }
    public override void RunActorSpecificAll<T>()
    {
        GameplayStateManager.Instance.RunActorSpecificAllTrigger<T>(ActorRetrieval, Trigger);
    }
}
