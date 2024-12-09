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
    
        public SerializedDictionary<StateContextTagScriptableObject, List<AbstractGameplayStateBehaviourScriptableObject>> LookForStates;
        
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

            foreach (StateContextTagScriptableObject contextTag in LookForStates.Keys.Where(contextTag => LookForStates[contextTag].Count != 0))
            {
                if (AllowRelations)
                {
                    status = actor.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state) &&
                             LookForStates[contextTag].Any(stateBehaviour => stateBehaviour.Get().Any(lState => lState.IsRelatedTo(state.StateData)));
                    // status = actor.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state) && LookForStates[contextTag].Any(state.StateData.IsRelatedTo);
                }

                if (AllowDescendants && !status)
                {
                    status = actor.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state) &&
                             LookForStates[contextTag].Any(stateBehaviour => stateBehaviour.Get().Any(lState => lState.IsDescendantOf(state.StateData)));
                }

                if (!status)
                {
                    status = actor.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state) && LookForStates[contextTag].Contains(state.StateData);
                }

                // If status and we don't want to check cached states then continue
                // This solves the edge case wherein undefined active states are not cached
                if (ActiveStatesOnly || status) continue;
                
                status = LookForStates[contextTag].Any(stateBehaviour => stateBehaviour.Get().Any(state => actor.Moderator.TryGetCachedState(contextTag, state, out _)));
                
                // LookForStates were not found in cached states, let's check relations & descendents of cached states
                if (AllowRelations && !status)
                {
                    status = LookForStates[contextTag].Any(stateBehaviour =>
                        stateBehaviour.Get().Any(state =>
                            actor.Moderator.TryGetCachedState(contextTag, state, out AbstractGameplayState storedState) && state.IsRelatedTo(storedState.StateData)));
                }

                if (AllowDescendants && !status)
                {
                    status = LookForStates[contextTag].Any(stateBehaviour =>
                        stateBehaviour.Get().Any(state =>
                            actor.Moderator.TryGetCachedState(contextTag, state, out AbstractGameplayState storedState) && state.IsDescendantOf(storedState.StateData)));
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
                status = LookForStates[contextTag].Any(stateBehaviour => stateBehaviour.Get().Any(state => state.IsRelatedTo(newState)));
            }
            if (AllowDescendants && !status)
            {
                status = LookForStates[contextTag].Any(stateBehaviour => stateBehaviour.Get().Any(state => state.IsRelatedTo(newState)));
            }
            if (!status)
            {
                status = LookForStates[contextTag].Contains(newState);
            }

            // If we haven't found the newState in LookFor and !ActiveStatesOnly, let's check to see if the LookFor states exist in cached States
            if (!status && !ActiveStatesOnly)
            {
                status = LookForStates[contextTag].Any(stateBehaviour => stateBehaviour.Get().Any(state => actor.Moderator.TryGetCachedState(contextTag, state, out AbstractGameplayState _)));
            }

            return status;
        }

        public override Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates()
        {
            Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> states =
                new Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>>();
            foreach (StateContextTagScriptableObject contextTag in LookForStates.Keys)
            {
                states[contextTag] = new List<AbstractGameplayStateScriptableObject>();
                foreach (AbstractGameplayStateBehaviourScriptableObject stateBehaviour in LookForStates[contextTag]) states[contextTag].AddRange(stateBehaviour.Get());
            }

            return states;
        }

        public override bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator)
        {
            return LookForModerators.Count <= 0 || LookForModerators.Contains(moderator);
        }
    }
}