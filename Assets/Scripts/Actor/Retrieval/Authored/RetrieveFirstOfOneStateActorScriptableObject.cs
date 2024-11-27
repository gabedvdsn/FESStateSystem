using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/First of One")]
public class RetrieveFirstOfOneStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    public StateIdentifierTagScriptableObject ActorTag;

    protected override T RetrieveActor<T>()
    {
        List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(ActorTag);
        if (actors is null || actors.Count == 0) return null;

        return actors[0];
    }

    protected override List<T> RetrieveManyActors<T>(int count)
    {
        List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(ActorTag);
        if (actors is null || actors.Count == 0) return null;
        
        int realCount = count < 0 ? actors.Count : Mathf.Min(count, actors.Count);
        return actors.Take(realCount).ToList();
    }
}
