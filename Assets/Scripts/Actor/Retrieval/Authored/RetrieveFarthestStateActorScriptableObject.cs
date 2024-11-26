using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FESState/Retrieval/Farthest From")]
public class RetrieveFarthestStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    [Header("Source Retrieval")] 
    public AbstractRetrieveStateActorScriptableObject SourceRetrieval;
    
    [Header("Target")]
    public StateIdentifierTagScriptableObject TargetIdentifier;
    
    public override bool TryRetrieveActor<T>(out T actor)
    {
        try
        {
            actor = RetrieveFarthestActor<T>();
            return actor is not null;
        }
        catch
        {
            actor = null;
            return false;
        }
    }
    
    private T RetrieveFarthestActor<T>() where T : StateActor
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;

        List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetIdentifier);
        if (targets is null || targets.Count == 0) return null;
        
        T farthest = null;
        float farthestDistance = float.MinValue;
        foreach (T target in targets)
        {
            float distance = Vector3.Distance(source.transform.position, target.transform.position);
            if (distance > farthestDistance)
            {
                farthest = target;
                farthestDistance = distance;
            }
        }

        return farthest;
    }

    public override bool TryRetrieveManyActors<T>(int count, out List<T> actors)
    {
        try
        {
            actors = RetrieveManyFarthestActors<T>(count);
            return actors is not null && actors.Count > 0;
        }
        catch
        {
            actors = null;
            return false;
        }
    }
    
    public override bool TryRetrieveAllActors<T>(out List<T> actors)
    {
        try
        {
            actors = RetrieveManyFarthestActors<T>(-1);
            return actors is not null && actors.Count > 0;
        }
        catch
        {
            actors = null;
            return false;
        }
    }
    
    private List<T> RetrieveManyFarthestActors<T>(int count) where T : StateActor
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;
        
        Vector3 origin = source.transform.position;
        
        List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetIdentifier);
        if (targets is null || targets.Count == 0) return null;
        
        int realCount = count < 0 ? targets.Count : Mathf.Min(count, targets.Count);
        return targets
            .OrderByDescending(t => Vector3.Distance(origin, t.transform.position))
            .Take(realCount)
            .ToList();
    }

    
}
