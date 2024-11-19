using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/Farthest From")]
public class RetrieveFarthestStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    [Header("Source Retrieval")] 
    public AbstractRetrieveStateActorScriptableObject SourceRetrieval;
    
    [Header("Target")]
    public GameplayStateTagScriptableObject TargetTag;
    
    public override T RetrieveActor<T>()
    {
        try
        {
            return RetrieveFarthestActor<T>();
        }
        catch
        {
            return null;
        }
    }
    
    private T RetrieveFarthestActor<T>() where T : StateActor
    {
        StateActor source = SourceRetrieval.RetrieveActor<StateActor>();
        if (source is null) return null;

        List<T> targets = GameplayStateManager.Instance.RetrieveActors<T>(TargetTag);
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

    public override List<T> RetrieveManyActors<T>(int count)
    {
        try
        {
            return RetrieveManyFarthestActors<T>(count);
        }
        catch
        {
            return null;
        }
    }
    
    public override List<T> RetrieveAllActors<T>()
    {
        try
        {
            return RetrieveManyFarthestActors<T>(-1);
        }
        catch
        {
            return null;
        }
    }
    
    private List<T> RetrieveManyFarthestActors<T>(int count) where T : StateActor
    {
        StateActor source = SourceRetrieval.RetrieveActor<StateActor>();
        if (source is null) return null;
        
        Vector3 origin = source.transform.position;
        
        List<T> targets = GameplayStateManager.Instance.RetrieveActors<T>(TargetTag);
        int realCount = count < 0 ? targets.Count : Mathf.Min(count, targets.Count);
        return targets
            .OrderByDescending(t => Vector3.Distance(origin, t.transform.position))
            .Take(realCount)
            .ToList();
    }

    
}
