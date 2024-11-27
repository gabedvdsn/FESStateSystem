using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRetrieveStateActorScriptableObject : ScriptableObject
{
    public virtual bool TryRetrieveActor<T>(out T actor) where T : StateActor
    {
        try
        {
            actor = RetrieveActor<T>();
            return actor is not null;
        }
        catch
        {
            actor = null;
            return false;
        }
    }

    public virtual bool TryRetrieveManyActors<T>(int count, out List<T> actors) where T : StateActor
    {
        try
        {
            actors = RetrieveManyActors<T>(count);
            return actors is not null && actors.Count > 0;
        }
        catch
        {
            actors = null;
            return false;
        }
    }

    public virtual bool TryRetrieveAllActors<T>(out List<T> actors) where T : StateActor
    {
        return TryRetrieveManyActors(-1, out actors);
    }

    protected abstract T RetrieveActor<T>() where T : StateActor;
    protected abstract List<T> RetrieveManyActors<T>(int count) where T : StateActor;
}
