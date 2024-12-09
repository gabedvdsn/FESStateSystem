using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/Trigger/State Trigger", fileName = "StateTrigger")]
    public class StateTriggerScriptableObject : AbstractStateTriggerScriptableObject
    {
        [Header("Moderator Trigger")]
        public StateModeratorScriptableObject OverrideModerator;

        [Space] 
        
        public bool TryPreserveAllStates;
        public List<StateContextTagScriptableObject> TryPreserveStates;
    
        [FormerlySerializedAs("FoceOverrideStates")] [Header("States Trigger")]
    
        public bool ForceOverrideStates;
        public SerializedDictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject> OverrideStates;
        public bool ReEnterSameOverrideStates;
    
        [Header("Other")]
        
        public bool ReEnterSameStates;

        /// <summary>
        /// Activates the state trigger WRT some State Actor
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="flag">Does the state change interrupt (true) or change normally (false)</param>
        /// <returns></returns>
        public override bool Activate(StateActor actor, bool flag)
        {
            if (actor is null) return false;
            
            // We want to change the actor's state moderator
            if (OverrideModerator)
            {
                // Let's implement the override moderator
                StateModerator.StateModeratorMeta overrideModerator = StateModerator.GenerateMeta(OverrideModerator);

                if (TryPreserveAllStates)
                {
                    Dictionary<StateContextTagScriptableObject, AbstractGameplayState> activeStates = actor.Moderator.GetActiveStatesWithContext();
                    // We want to preserve states at all priorities
                    foreach (StateContextTagScriptableObject contextTag in activeStates.Keys)
                    {
                        // Ensure the override moderator defines the current state
                        if (!overrideModerator.DefinesState(contextTag, activeStates[contextTag].StateData)) continue;
                        overrideModerator.ChangeInitialState(contextTag, activeStates[contextTag].StateData);
                    }
                }
                else if (TryPreserveStates.Count > 0)
                {
                    // We want to preserve state(s) at some priorities
                    foreach (StateContextTagScriptableObject contextTag in TryPreserveStates)
                    {
                        // Ensure the override moderator defines the current state
                        if (actor.Moderator.TryGetActiveState(contextTag, out AbstractGameplayState state) && !overrideModerator.DefinesState(contextTag, state.StateData)) continue;
                        overrideModerator.ChangeInitialState(contextTag, state.StateData);
                    }
                }
            
                foreach (StateContextTagScriptableObject contextTag in OverrideStates.Keys)
                {
                    if (!overrideModerator.DefinesState(contextTag, OverrideStates[contextTag]) && !ForceOverrideStates) continue;
                    overrideModerator.ChangeInitialState(contextTag, OverrideStates[contextTag]);
                }
            
                actor.Moderator.ImplementModeratorMeta(overrideModerator, ReEnterSameStates, flag);
                return true;
            }
        
            if (OverrideStates is not null)
            {
                Dictionary<StateContextTagScriptableObject, AbstractGameplayState> activeStates = actor.Moderator.GetActiveStatesWithContext();
                
                if (ReEnterSameStates)
                {
                    foreach (StateContextTagScriptableObject contextTag in activeStates.Keys)
                    {
                        if (OverrideStates.Keys.Contains(contextTag)) continue;
                    
                        ChangeState(actor.Moderator, contextTag, activeStates[contextTag].StateData, flag);
                    }
                }
                
                foreach (StateContextTagScriptableObject contextTag in OverrideStates.Keys)
                {
                    if (!actor.Moderator.DefinesState(contextTag, OverrideStates[contextTag]) && !ForceOverrideStates) continue;
                    if (OverrideStates[contextTag] == activeStates[contextTag].StateData && !ReEnterSameOverrideStates) continue; 

                    ChangeState(actor.Moderator, contextTag, OverrideStates[contextTag], flag);
                }

                return true;
            }
        
            if (ReEnterSameStates)
            {
                Dictionary<StateContextTagScriptableObject, AbstractGameplayState> activeStates = actor.Moderator.GetActiveStatesWithContext();
                foreach (StateContextTagScriptableObject contextTag in activeStates.Keys)
                {
                    ChangeState(actor.Moderator, contextTag, activeStates[contextTag].StateData, flag);
                }
                return true;
            }

            return false;
        }

        private void ChangeState(StateModerator moderator, StateContextTagScriptableObject contextTag,
            AbstractGameplayStateScriptableObject state, bool interrupts)
        {
            if (interrupts) moderator.InterruptChangeState(contextTag, state, !ForceOverrideStates);
            else moderator.DefaultChangeState(contextTag, state, !ForceOverrideStates);
        }
    
        protected virtual void OnValidate()
        {
            if (!OverrideModerator && OverrideStates.Count == 0 && !ReEnterSameStates) throw new Exception($"({name}) State Trigger must define either (or both) Override Moderator or Transition State, or ReEnterSameStates must be true (performs simple moderator reset)");
        }
    }
}