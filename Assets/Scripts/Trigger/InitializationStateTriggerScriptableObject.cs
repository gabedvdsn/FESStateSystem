using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/Trigger/Initialization Trigger")]
    public class InitializationStateTriggerScriptableObject : AbstractStateTriggerScriptableObject
    {
        [Header("Initialization")]
        public StateModeratorScriptableObject InitialModerator;
        public SerializedDictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject> OverrideStates;
    
        public override bool Activate(StateActor actor, bool flag)
        {
            actor.Moderator = InitialModerator.GenerateModerator(actor);
        
            foreach (StateContextTagScriptableObject priorityTag in OverrideStates.Keys)
            {
                if (!actor.Moderator.DefinesState(priorityTag, OverrideStates[priorityTag])) continue;
                actor.Moderator.DefaultChangeState(priorityTag, OverrideStates[priorityTag]);
            }

            return true;
        }
    
        protected void OnValidate()
        {
            if (!InitialModerator) throw new Exception("Initialization State Trigger must define Override Moderator");
        }
    }
}