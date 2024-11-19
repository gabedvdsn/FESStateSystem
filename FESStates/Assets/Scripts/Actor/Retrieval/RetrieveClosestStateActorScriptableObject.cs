using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FESState/Retrieval/Closest To")]
public class RetrieveClosestStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    [Header("Source Retrieval")] 
    public AbstractRetrieveStateActorScriptableObject SourceRetrieval;
    
    [Header("Target")]
    public GameplayStateTagScriptableObject TargetTag;
    
    public override T RetrieveActor<T>()
    {
        try
        {
            return RetrieveClosestActor<T>();
        }
        catch
        {
            return null;
        }
    }

    private T RetrieveClosestActor<T>() where T : StateActor
    {
        StateActor source = SourceRetrieval.RetrieveActor<StateActor>();
        if (source is null) return null;

        List<T> targets = GameplayStateManager.Instance.RetrieveActors<T>(TargetTag);
        T closest = null;
        float closestDistance = float.MaxValue;
        foreach (T target in targets)
        {
            float distance = Vector3.Distance(source.transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closest = target;
                closestDistance = distance;
            }
        }

        return closest;
    }
}
