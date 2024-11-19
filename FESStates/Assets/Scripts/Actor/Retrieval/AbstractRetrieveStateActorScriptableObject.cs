using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRetrieveStateActorScriptableObject : ScriptableObject
{
    public abstract T RetrieveActor<T>() where T : StateActor;

    public abstract List<T> RetrieveManyActors<T>(int count) where T : StateActor;

    public abstract List<T> RetrieveAllActors<T>() where T : StateActor;
}
