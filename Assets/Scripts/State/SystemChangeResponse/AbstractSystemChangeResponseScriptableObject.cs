using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    public abstract class AbstractSystemChangeResponseScriptableObject : ScriptableObject
    {
        public AbstractStateConditionalTriggerScriptableObject FromConditional;
        public AbstractStateConditionalTriggerScriptableObject ToConditional;
    
        [Space]
    
        public bool SkipChangeToSame = true;

        public void OnModeratorChanged(StateActor actor, StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
        {
            if (oldModerator == newModerator && SkipChangeToSame) return;
        
            if (FromConditional)
            {
                if (!FromConditional.Activate(actor, true)) return;
            }

            if (ToConditional)
            {
                if (!ToConditional.PreModeratorChangeActivate(newModerator)) return;
            }
        
            ModeratorChangeResponse(actor, oldModerator, newModerator);
        }
    
        public void OnStateChanged(StateActor actor, StatePriorityTagScriptableObject priorityTag, AbstractGameplayState oldState, AbstractGameplayState newState)
        {
            if (oldState?.StateData == newState.StateData && SkipChangeToSame) return;
        
            if (FromConditional)
            {
                if (!FromConditional.Activate(actor, true)) return;
            }

            if (ToConditional)
            {
                if (!ToConditional.PreStateChangeActivate(actor, priorityTag, newState.StateData)) return;
            }
        
            StateChangeResponse(actor, oldState, newState, FromConditional, ToConditional);
        
        }

        protected abstract void ModeratorChangeResponse(StateActor actor, StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator);
    
        protected abstract void StateChangeResponse(StateActor actor, AbstractGameplayState oldState, AbstractGameplayState newState, AbstractStateConditionalTriggerScriptableObject fromConditional, AbstractStateConditionalTriggerScriptableObject toConditional);
    }
}