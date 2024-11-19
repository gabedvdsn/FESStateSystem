using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Retrieval/System Specific")]
public class RetrieveSystemSpecificStateActorScriptableObject : AbstractRetrieveStateActorScriptableObject
{
    [Header("Source Retrieval [Can Be Null]")] 
    
    [Tooltip("If null, retrieves the first actor that fits criteria from every subscribed actor. Be careful about infinite retrieval cycles.")]
    public AbstractRetrieveStateActorScriptableObject SourceRetrieval;

    [Header("Moderator")]
    public List<StateModeratorScriptableObject> LookForModerator;

    [Header("State")] 
    public bool ActiveStatesOnly = true;
    public SerializedDictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> LookForState;
    
    public override T RetrieveActor<T>()
    {
        try
        {
            return SourceRetrieval ? RetrieveBySource(SourceRetrieval.RetrieveActor<T>()) : RetrieveByAllActors<T>();
        }
        catch
        {
            return null;
        }
    }

    private T RetrieveByAllActors<T>() where T : StateActor
    {
        Dictionary<GameplayStateTagScriptableObject, List<StateActor>> allActors = GameplayStateManager.Instance.AllActors;
        foreach (GameplayStateTagScriptableObject actorTag in allActors.Keys)
        {
            foreach (StateActor actor in allActors[actorTag])
            {
                if (ValidateModerator(actor) && ValidateStates(actor)) return actor as T;
            }
        }

        return null;
    }

    private T RetrieveBySource<T>(T source) where T : StateActor
    {
        if (ValidateModerator(source) && ValidateStates(source)) return source;
        return null;
    }

    private bool ValidateModerator(StateActor actor)
    {
        if (LookForModerator is not null && LookForModerator.Count > 0) return LookForModerator.Any(m => actor.Moderator.BaseModerator == m);

        return true;
    }

    private bool ValidateStates(StateActor actor)
    {
        if (ActiveStatesOnly)
        {
            Dictionary<StatePriorityTagScriptableObject, AbstractGameplayState> activeStates = actor.Moderator.GetActiveStates();
            foreach (StatePriorityTagScriptableObject priorityTag in LookForState.Keys)
            {
                if (!activeStates.ContainsKey(priorityTag)) continue;
                if (LookForState[priorityTag].All(s => s != activeStates[priorityTag].StateData)) return false;
            }
        }
        else
        {
            foreach (StatePriorityTagScriptableObject priorityTag in LookForState.Keys)
            {
                if (!LookForState[priorityTag].All(s => actor.Moderator.DefinesState(priorityTag, s))) return false;
            }
        }

        return true;

    }

    private void OnValidate()
    {
        if (SourceRetrieval == this) SourceRetrieval = null;
    }
}
