using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    public abstract class AbstractGameplayStateScriptableObject : AbstractGameplayStateBehaviourScriptableObject
    {
        [Header("Gameplay State")] 
        public AbstractGameplayStateScriptableObject Parent;
        public StateTriggerScriptableObject OnConcludeTrigger;

        [Space]

        public bool CacheState;
        
        [Header("Transition Behaviour")]
        public int SelectionPriority = 0;

        public override bool Defines(AbstractGameplayStateScriptableObject state)
        {
            return state == this;
        }

        public override List<AbstractGameplayStateScriptableObject> Get()
        {
            return new List<AbstractGameplayStateScriptableObject>()
            {
                this
            };
        }

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