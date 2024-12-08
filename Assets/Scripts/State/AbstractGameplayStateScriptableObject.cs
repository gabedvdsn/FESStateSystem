using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractGameplayStateScriptableObject : AbstractGameplayStateBehaviourScriptableObject
    {
        public AbstractGameplayStateScriptableObject Parent;
        public StateTriggerScriptableObject ConclusionTrigger;

        [Space]

        public bool CacheState;

        public override AbstractGameplayStateScriptableObject Initial()
        {
            return this;
        }

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