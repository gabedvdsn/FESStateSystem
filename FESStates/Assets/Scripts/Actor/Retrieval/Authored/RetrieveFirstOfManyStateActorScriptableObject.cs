
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/First of Many")]
public class RetrieveFirstOfManyStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    public List<GameplayStateTagScriptableObject> FirstTagInOrder;
    
    public override bool TryRetrieveActor<T>(out T actor)
    {
        try
        {
            actor = RetrieveFirstOfManyActor<T>();
            return actor is not null;
        }
        catch
        {
            actor = null;
            return false;
        }
    }

    private T RetrieveFirstOfManyActor<T>() where T : StateActor
    {
        foreach (GameplayStateTagScriptableObject actorTag in FirstTagInOrder)
        {
            List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(actorTag);
            if (actors is not null && actors.Count > 0)
            {
                return actors[0];
            }
        }

        return null;
    }

    public override bool TryRetrieveManyActors<T>(int count, out List<T> actors)
    {
        try
        {
            actors = RetrieveManyFirstOfManyActors<T>(count);
            return actors is not null;
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
            actors = RetrieveManyFirstOfManyActors<T>(-1);
            return actors is not null;
        }
        catch
        {
            actors = null;
            return false;
        }
    }

    private List<T> RetrieveManyFirstOfManyActors<T>(int count) where T : StateActor
    {
        foreach (GameplayStateTagScriptableObject actorTag in FirstTagInOrder)
        {
            List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(actorTag);
            if (actors is not null && actors.Count > 0)
            {
                int realCount = count < 0 ? actors.Count : Mathf.Min(count, actors.Count);
                return actors.Take(realCount).ToList();
            }
        }

        return null;
    }
}
