using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/System Change Response/Debug")]
    public class DebugSystemChangeResponse : AbstractSystemChangeResponseScriptableObject
    {
        protected override void ModeratorChangeResponse(StateActor actor, StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
        {
            Debug.Log($"[ MODIFIER CHANGED RESPONSE ] [ {actor.name} ]\n\tFROM {oldModerator.name} TO {newModerator.name}");
        }
    
        protected override void StateChangeResponse(StateActor actor, AbstractGameplayState oldState, AbstractGameplayState newState, AbstractStateConditionalTriggerScriptableObject fromConditional, AbstractStateConditionalTriggerScriptableObject toConditional)
        {
            string message = $"[ STATE CHANGED RESPONSE ] [ {actor.name} ] ";

            Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> fromStates;
            Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> toStates;

            if (fromConditional) fromStates = fromConditional.GetStates();
            else
            {
                if (oldState is null || actor.Moderator is null) fromStates = new();
                else
                {
                    fromStates = new Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>>()
                    {
                        { oldState.LiveContext, new List<AbstractGameplayStateScriptableObject>() { oldState.StateData } }
                    };
                }
            }

            if (toConditional) toStates = toConditional.GetStates();
            else
            {
                if (newState is null || actor.Moderator is null) toStates = new();
                else
                {
                    toStates = new Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>>()
                    {
                        { newState.LiveContext, new List<AbstractGameplayStateScriptableObject>() { newState.StateData } }
                    };
                }
            }
        
            // Priority tags unique to fromState
            foreach (StateContextTagScriptableObject contextTag in fromStates.Keys.Where(contextTag => !toStates.ContainsKey(contextTag)))
            {
                foreach (AbstractGameplayStateScriptableObject state in fromStates[contextTag]) message += $"\n\t[ {contextTag.Priority} ] FROM {state.name}";
            }
        
            // Priority tags at intersection of fromState and toState
            foreach (StateContextTagScriptableObject contextTag in fromStates.Keys.Where(contextTag => toStates.ContainsKey(contextTag)))
            {
                message += $"\n\t[ {contextTag.Priority} ] FROM {fromStates[contextTag][0].name} TO {toStates[contextTag][0].name}";
            }

            // Priority tags unique to toState
            foreach (StateContextTagScriptableObject contextTag in toStates.Keys.Where(contextTag => !fromStates.ContainsKey(contextTag)))
            {
                foreach (AbstractGameplayStateScriptableObject state in toStates[contextTag]) message += $"\n\t[ {contextTag.Priority} ] TO {state.name}";
            }
        
            Debug.Log(message);
        }
    }
}