using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/First of One")]
public class RetrieveFirstOfOneStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    public StateIdentifierTagScriptableObject ActorTag;
    
    public override bool TryRetrieveActor<T>(out T actor)
    {
        try
        {
            actor = RetrieveFirstOfExactActor<T>();
            return actor is not null;
        }
        catch
        {
            actor = null;
            return false;
        }
    }
    public override bool TryRetrieveManyActors<T>(int count, out List<T> actors)
    {
        try
        {
            actors = RetrieveManyFirstOfExactActor<T>(count);
            return actors is not null && actors.Count > 0;
        }
        catch
        {
            actors = null;
            return false;
        }
    }
    public override bool TryRetrieveAllActors<T>(out List<T> actors)
    {
        try
        {
            actors = RetrieveManyFirstOfExactActor<T>(-1);
            return actors is not null && actors.Count > 0;
        }
        catch
        {
            actors = null;
            return false;
        }
    }

    private T RetrieveFirstOfExactActor<T>() where T : StateActor
    {
        List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(ActorTag);
        if (actors is null || actors.Count == 0) return null;

        return actors[0];
    }

    private List<T> RetrieveManyFirstOfExactActor<T>(int count) where T : StateActor
    {
        List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(ActorTag);
        if (actors is null || actors.Count == 0) return null;
        
        int realCount = count < 0 ? actors.Count : Mathf.Min(count, actors.Count);
        return actors.Take(realCount).ToList();
    }
}
