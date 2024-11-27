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
    public StateIdentifierTagScriptableObject TargetTag;
    
    protected override T RetrieveActor<T>()
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;

        List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetTag);
        if (targets is null || targets.Count == 0) return null;
        
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

    protected override List<T> RetrieveManyActors<T>(int count)
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;
        
        Vector3 origin = source.transform.position;
        
        List<T> targets = GameplayStateManager.Instance.RetrieveActorsByTag<T>(TargetTag);
        if (targets is null || targets.Count == 0) return null;
        
        int realCount = count < 0 ? targets.Count : Mathf.Min(count, targets.Count);
        return targets
            .OrderBy(t => Vector3.Distance(origin, t.transform.position))
            .Take(realCount)
            .ToList();
    }
    
}
