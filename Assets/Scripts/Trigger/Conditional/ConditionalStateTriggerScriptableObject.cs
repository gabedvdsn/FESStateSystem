using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger", fileName = "ConditionalTrigger")]
    public class ConditionalStateTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject
    {
        [Header("Moderator")]
    
        public List<StateModeratorScriptableObject> LookForModerators;
    
        [Header("States")]
    
        public SerializedDictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> LookForStates;
        
        public bool ActiveStatesOnly = true;
        public bool AllowDescendants;
        public bool AllowRelations;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor">The actor to activate the trigger on</param>
        /// <param name="flag">No meaning for conditional triggers</param>
        /// <returns></returns>
        public override bool Activate(StateActor actor, bool flag)
        {
            if (actor.Moderator is null) return false;
            if (LookForModerators.Count > 0 && !LookForModerators.Contains(actor.Moderator.BaseModerator)) return false;

            bool status = LookForStates.Count == 0;
        
            foreach (StateContextTagScriptableObject priorityTag in LookForStates.Keys.Where(priorityTag => LookForStates[priorityTag].Count != 0))
            {
                if (ActiveStatesOnly)
                {
                    if (AllowRelations)
                    {
                        status = actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && LookForStates[priorityTag].Any(state.StateData.IsRelatedTo);
                    }
                    if (AllowDescendants && !status)
                    {
                        status = actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && LookForStates[priorityTag].Any(state.StateData.IsDescendantOf);
                    }
                    if (!status)
                    {
                        status = actor.Moderator.TryGetActiveState(priorityTag, out AbstractGameplayState state) && LookForStates[priorityTag].Contains(state.StateData);
                    }
                }
                else
                {
                    status = LookForStates[priorityTag].Any(state => actor.Moderator.TryGetCachedState(priorityTag, state, out _));
                
                    if (status) continue;
                    
                    // LookForStates were not found in stored states, let's check relations & descendents of stored states
                    if (AllowRelations)
                    {
                        status = LookForStates[priorityTag].Any(state =>
                            actor.Moderator.TryGetCachedState(priorityTag, state, out AbstractGameplayState storedState) && state.IsRelatedTo(storedState.StateData));
                    }
                    
                    if (AllowDescendants && !status)
                    {
                        status = LookForStates[priorityTag].Any(state =>
                            actor.Moderator.TryGetCachedState(priorityTag, state, out AbstractGameplayState storedState) && state.IsDescendantOf(storedState.StateData));
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Finds the newState in LookForStates, or within the actor's stored states if not ActiveOnly
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="contextTag"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        public override bool PreStateChangeActivate(StateActor actor, StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState)
        {
            if (!LookForStates.ContainsKey(contextTag)) return false;
        
            bool status = false;
            if (AllowRelations)
            {
                status = LookForStates[contextTag].Any(newState.IsRelatedTo);
            }
            if (AllowDescendants && !status)
            {
                status = LookForStates[contextTag].Any(newState.IsDescendantOf);
            }
            if (!status)
            {
                status = LookForStates[contextTag].Contains(newState);
            }

            // If we haven't found the newState in LookFor and !ActiveStatesOnly, let's check to see if the LookFor states exist in Stored States
            if (!status && !ActiveStatesOnly)
            {
                status = LookForStates[contextTag].Any(s => actor.Moderator.TryGetCachedState(contextTag, s, out AbstractGameplayState _));
            }

            return status;
        }
    
        public override Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates() => LookForStates;

        public override bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator)
        {
            return LookForModerators.Count <= 0 || LookForModerators.Contains(moderator);
        }
    }
}