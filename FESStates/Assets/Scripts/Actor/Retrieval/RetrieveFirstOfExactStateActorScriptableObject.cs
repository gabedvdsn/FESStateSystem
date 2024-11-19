using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/First of Exact")]
public class RetrieveFirstOfExactStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    public GameplayStateTagScriptableObject ActorTag;
    
    public override T RetrieveActor<T>()
    {
        try
        {
            return RetrieveFirstOfExactActor<T>();
        }
        catch
        {
            return null;
        }
    }

    private T RetrieveFirstOfExactActor<T>() where T : StateActor
    {
        List<T> actors = GameplayStateManager.Instance.RetrieveActors<T>(ActorTag);
        if (actors is null || actors.Count == 0) return null;

        return actors[0];
    }
}
