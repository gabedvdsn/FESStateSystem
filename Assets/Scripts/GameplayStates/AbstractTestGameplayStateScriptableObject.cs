using UnityEngine;


public abstract class AbstractTestGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    
    public abstract class AbstractTestGameplayState : AbstractGameplayState
    {
        public AbstractTestGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {
            
        }
        
    }
}

