
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/First of Many")]
public class RetrieveFirstOfManyStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    public List<StateIdentifierTagScriptableObject> FirstTagInOrder;

    protected override T RetrieveActor<T>()
    {
        foreach (StateIdentifierTagScriptableObject actorTag in FirstTagInOrder)
        {
            List<T> actors = GameplayStateManager.Instance.RetrieveActorsByTag<T>(actorTag);
            if (actors is not null && actors.Count > 0)
            {
                return actors[0];
            }
        }

        return null;
    }

    protected override List<T> RetrieveManyActors<T>(int count)
    {
        foreach (StateIdentifierTagScriptableObject actorTag in FirstTagInOrder)
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
