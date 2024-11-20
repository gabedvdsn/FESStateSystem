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
    
    public override bool TryRetrieveActor<T>(out T actor)
    {
        try
        {
            actor = RetrieveClosestActor<T>();
            return actor is not null;
        }
        catch
        {
            actor = null;
            return false;
        }
    }
    
    private T RetrieveClosestActor<T>() where T : StateActor
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;

        List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetTag);
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
    
    public override bool TryRetrieveManyActors<T>(int count, out List<T> actors)
    {
        try
        {
            actors = RetrieveManyClosestActors<T>(count);
            return actors is not null;
        }
        catch
        {
            actors = null;
            return false;
        }
    }

    private List<T> RetrieveManyClosestActors<T>(int count) where T : StateActor
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;
        
        Vector3 origin = source.transform.position;
        
        List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetTag);
        int realCount = count < 0 ? targets.Count : Mathf.Min(count, targets.Count);
        return targets
            .OrderBy(t => Vector3.Distance(origin, t.transform.position))
            .Take(realCount)
            .ToList();
    }
    
    public override bool TryRetrieveAllActors<T>(out List<T> actors)
    {
        try
        {
            actors = RetrieveManyClosestActors<T>(-1);
            return actors is not null;
        }
        catch
        {
            actors = null;
            return false;
        }
    }

    
}
