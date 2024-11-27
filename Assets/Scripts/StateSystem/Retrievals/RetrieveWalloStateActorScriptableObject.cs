using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/Wallo")]
public class RetrieveWalloStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    
    [Header("Source Retrieval")] 
    public AbstractRetrieveStateActorScriptableObject SourceRetrieval;

    [Header("Targets")]
    public List<StateIdentifierTagScriptableObject> TargetIdentifiers;

    // Implement behaviour here
    protected override T RetrieveActor<T>()
    {
        if (!SourceRetrieval.TryRetrieveActor(out StateActor source)) return null;

		if (TargetIdentifiers is null || TargetIdentifiers.Count == 0) return null;

		throw new System.NotImplementedException();
    }

    // Implement behaviour here
    // Such that count < 0 should retrieve all actors
    protected override List<T> RetrieveManyActors<T>(int count)
    {
        if (!SourceRetrieval.TryRetrieveManyActors(count, out List<StateActor> sources)) return null;

		if (TargetIdentifiers is null || TargetIdentifiers.Count == 0) return null;

		throw new System.NotImplementedException();
    }  
    
}

