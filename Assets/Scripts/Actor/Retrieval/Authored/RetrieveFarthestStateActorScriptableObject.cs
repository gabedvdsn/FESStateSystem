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
    
    protected override T RetrieveActor<T>()
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
    
    protected override List<T> RetrieveManyActors<T>(int count)
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
