using System.Collections.Generic;
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
}
