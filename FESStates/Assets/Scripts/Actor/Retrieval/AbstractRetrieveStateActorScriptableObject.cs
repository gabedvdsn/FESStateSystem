using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRetrieveStateActorScriptableObject : ScriptableObject
{
    public abstract T RetrieveActor<T>() where T : StateActor;
}
