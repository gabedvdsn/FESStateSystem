using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger")]
public class ConditionalStateTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject
{
    [Header("Moderator")]
    
    public List<StateModeratorScriptableObject> LookForModerators;
    
    [Header("States")]
    
    public SerializedDictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> LookForStates;

    [Tooltip("Any conditional can evaluate to true, as opposed to all of them")]
    public bool LookForAny = true;
    public bool ActiveStatesOnly = true;
    public bool AllowDescendants;
    public bool AllowRelations;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    public override bool Activate(StateActor actor, bool flag)
    {
        if (actor.Moderator is null) return false;
        if (LookForModerators.Count > 0 && !LookForModerators.Contains(actor.Moderator.BaseModerator)) return false;

        bool status = LookForStates.Count == 0;
        
        foreach (StatePriorityTagScriptableObject priorityTag in LookForStates.Keys.Where(priorityTag => LookForStates[priorityTag].Count != 0))
        {
            if (ActiveStatesOnly)
            {
                if (AllowRelations)
                {
                    status = actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && (LookForAny ? LookForStates[priorityTag].Any(state.StateData.IsRelatedTo) : LookForStates[priorityTag].All(state.StateData.IsRelatedTo));
                }
                if (AllowDescendants && !status)
                {
                    status = actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && (LookForAny ? LookForStates[priorityTag].Any(state.StateData.IsDescendantOf) : LookForStates[priorityTag].All(state.StateData.IsDescendantOf));
                }
                if (!status)
                {
                    status = actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && (LookForAny ? LookForStates[priorityTag].Contains(state.StateData) : LookForStates[priorityTag].All(s => s == state.StateData));
                }
            }
            else
            {
                status = LookForAny ? LookForStates[priorityTag].Any(state => actor.Moderator.TryGetStoredState(priorityTag, state, out _)) : LookForStates[priorityTag].All(state => actor.Moderator.TryGetStoredState(priorityTag, state, out _));
                
                // LookForStates were not found in stored states, let's check relations & descendents of stored states
                if (!status)
                {
                    if (AllowRelations)
                    {
                        // Any or all the conditions have relations in stored states
                        status = LookForAny
                            ? LookForStates[priorityTag].Any(state =>
                                actor.Moderator.TryGetStoredState(priorityTag, state, out AbstractGameplayState storedState) && state.IsRelatedTo(storedState.StateData))
                            : LookForStates[priorityTag].All(state =>
                                actor.Moderator.TryGetStoredState(priorityTag, state, out AbstractGameplayState storedState) && state.IsRelatedTo(storedState.StateData));
                    }
                    
                    if (AllowDescendants && !status)
                    {
                        // Any or all the conditions have descendents in stored states
                        status = LookForAny
                            ? LookForStates[priorityTag].Any(state =>
                                actor.Moderator.TryGetStoredState(priorityTag, state, out AbstractGameplayState storedState) && state.IsDescendantOf(storedState.StateData))
                            : LookForStates[priorityTag].All(state =>
                                actor.Moderator.TryGetStoredState(priorityTag, state, out AbstractGameplayState storedState) && state.IsDescendantOf(storedState.StateData));
                    }
                }
            }
        }

        return status;
    }

    /// <summary>
    /// Finds the newState in LookForStates, or within the actor's stored states if not ActiveOnly
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="priorityTag"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public override bool PreStateChangeActivate(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState)
    {
        if (!LookForStates.ContainsKey(priorityTag)) return false;
        
        bool status = false;
        if (AllowRelations)
        {
            status = LookForAny ? LookForStates[priorityTag].Any(newState.IsRelatedTo) : LookForStates[priorityTag].All(newState.IsRelatedTo);
        }
        if (AllowDescendants && !status)
        {
            status = LookForAny ? LookForStates[priorityTag].Any(newState.IsDescendantOf) : LookForStates[priorityTag].All(newState.IsDescendantOf);
        }
        if (!status)
        {
            status = LookForAny ? LookForStates[priorityTag].Contains(newState) : LookForStates[priorityTag].All(s => s == newState);
        }

        // If we haven't found the newState in LookFor and !ActiveStatesOnly, let's check to see if the LookFor states exist in Stored States
        if (!status && !ActiveStatesOnly)
        {
            if (LookForAny) status = LookForStates[priorityTag].Any(s => actor.Moderator.TryGetStoredState(priorityTag, s, out AbstractGameplayState _));
            else status = LookForStates[priorityTag].All(s => actor.Moderator.TryGetStoredState(priorityTag, s, out AbstractGameplayState _));
        }

        return status;
    }
    
    public override Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates() => LookForStates;

    public override bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator)
    {
        return LookForModerators.Count <= 0 || LookForModerators.Contains(moderator);
    }
}