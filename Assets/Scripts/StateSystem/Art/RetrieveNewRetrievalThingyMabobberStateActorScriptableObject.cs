using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/New Retrieval Thingy Mabobber")]
public class RetrieveNewRetrievalThingyMabobberStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    
    // Implement behaviour here
    protected override T RetrieveActor<T>()
    {
        throw new System.NotImplementedException();
    }

    // Implement behaviour here
    // Such that count < 0 should retrieve all actors
    protected override List<T> RetrieveManyActors<T>(int count)
    {
        throw new System.NotImplementedException();
    }  
    
}

