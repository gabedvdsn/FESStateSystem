using System.Collections.Generic;
using System.Linq;
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
    
    public override List<T> RetrieveManyActors<T>(int count)
    {
        try
        {
            return RetrieveManyClosestActors<T>(count);
        }
        catch
        {
            return null;
        }
    }

    private List<T> RetrieveManyClosestActors<T>(int count) where T : StateActor
    {
        StateActor source = SourceRetrieval.RetrieveActor<StateActor>();
        if (source is null) return null;
        
        Vector3 origin = source.transform.position;
        
        List<T> targets = GameplayStateManager.Instance.RetrieveActors<T>(TargetTag);
        int realCount = count < 0 ? targets.Count : Mathf.Min(count, targets.Count);
        return targets
            .OrderBy(t => Vector3.Distance(origin, t.transform.position))
            .Take(realCount)
            .ToList();
    }
    
    public override List<T> RetrieveAllActors<T>()
    {
        try
        {
            return RetrieveManyClosestActors<T>(-1);
        }
        catch
        {
            return null;
        }
    }

    
}
