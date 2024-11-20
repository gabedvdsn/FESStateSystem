using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRetrieveStateActorScriptableObject : ScriptableObject
{
    public abstract bool TryRetrieveActor<T>(out T actor) where T : StateActor;

    public abstract bool TryRetrieveManyActors<T>(int count, out List<T> actors) where T : StateActor;

    public abstract bool TryRetrieveAllActors<T>(out List<T> actors) where T : StateActor;
}
