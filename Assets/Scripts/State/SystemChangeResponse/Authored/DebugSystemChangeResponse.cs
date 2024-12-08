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

            Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> fromStates;
            Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>> toStates;

            if (fromConditional) fromStates = fromConditional.GetStates();
            else
            {
                if (oldState is null || actor.Moderator is null) fromStates = new();
                else if (!actor.Moderator.TryGetActiveStatePriority(oldState.StateData, out StatePriorityTagScriptableObject priorityTag)) fromStates = new();
                else
                {
                    fromStates = new Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>>()
                    {
                        { priorityTag, new List<AbstractGameplayStateScriptableObject>() { oldState.StateData } }
                    };
                }
            }

            if (toConditional) toStates = toConditional.GetStates();
            else
            {
                if (newState is null || actor.Moderator is null) toStates = new();
                else if (!actor.Moderator.TryGetActiveStatePriority(newState.StateData, out StatePriorityTagScriptableObject priorityTag)) toStates = new();
                else
                {
                    toStates = new Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>>()
                    {
                        { priorityTag, new List<AbstractGameplayStateScriptableObject>() { newState.StateData } }
                    };
                }
            }
        
            // Priority tags unique to fromState
            foreach (StatePriorityTagScriptableObject priorityTag in fromStates.Keys.Where(p => !toStates.ContainsKey(p)))
            {
                foreach (AbstractGameplayStateScriptableObject state in fromStates[priorityTag]) message += $"\n\t[ {priorityTag.Priority} ] FROM {state.name}";
            }
        
            // Priority tags at intersection of fromState and toState
            foreach (StatePriorityTagScriptableObject priorityTag in fromStates.Keys.Where(p => toStates.ContainsKey(p)))
            {
                message += $"\n\t[ {priorityTag.Priority} ] FROM {fromStates[priorityTag][0].name} TO {toStates[priorityTag][0].name}";
            }

            // Priority tags unique to toState
            foreach (StatePriorityTagScriptableObject priorityTag in toStates.Keys.Where(p => !fromStates.ContainsKey(p)))
            {
                foreach (AbstractGameplayStateScriptableObject state in toStates[priorityTag]) message += $"\n\t[ {priorityTag.Priority} ] TO {state.name}";
            }
        
            Debug.Log(message);
        }
    }
}