
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/First of Many")]
public class RetrieveFirstOfManyStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    public List<GameplayStateTagScriptableObject> FirstTagInOrder;
    
    public override T RetrieveActor<T>()
    {
        try
        {
            return RetrieveFirstOfManyActor<T>();
        }
        catch
        {
            return null;
        }
    }

    private T RetrieveFirstOfManyActor<T>() where T : StateActor
    {
        foreach (GameplayStateTagScriptableObject actorTag in FirstTagInOrder)
        {
            List<T> actors = GameplayStateManager.Instance.RetrieveActors<T>(actorTag);
            if (actors is not null && actors.Count > 0)
            {
                return actors[0];
            }
        }

        return null;
    }
}
