using UnityEngine;

public abstract class AbstractTriggerRunner : MonoBehaviour
{
    public abstract void RunDefault();
    public abstract void RunDefaultMany(int count = -1);
    public abstract void RunDefaultAll();
    public abstract void RunActorSpecific<T>() where T : StateActor;
    public abstract void RunActorSpecificMany<T>(int count = -1) where T : StateActor;
    public abstract void RunActorSpecificAll<T>() where T : StateActor;
}
