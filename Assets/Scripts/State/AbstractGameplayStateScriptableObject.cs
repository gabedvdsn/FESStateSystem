using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractGameplayStateScriptableObject : ScriptableObject
    {
        public AbstractGameplayStateScriptableObject Parent;
        public StateTriggerScriptableObject ConclusionTrigger;
    
        public abstract AbstractGameplayState GenerateState(StateActor actor);

        public bool IsDescendantOf(AbstractGameplayStateScriptableObject other)
        {
            AbstractGameplayStateScriptableObject parent = Parent;
            while (parent is not null)
            {
                if (parent == other) return true;
                parent = parent.Parent;
            }

            return false;
        }

        public bool IsRelatedTo(AbstractGameplayStateScriptableObject other)
        {
            if (other == this) return true;
        
            AbstractGameplayStateScriptableObject parent = Parent;
            while (parent is not null)
            {
                if (parent == other) return true;
                if (other.IsDescendantOf(parent)) return true;
                parent = parent.Parent;
            }

            return false;
        }
    }
}