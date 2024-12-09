using UnityEngine;
using System.Collections.Generic;
using FESStateSystem;

[CreateAssetMenu(menuName = "FESState/Authored/State/General/Kitchen State")]
public class KitchenGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    
    public override List<AbstractGameplayState> GenerateStates(StateActor actor)
    {
        return new List<AbstractGameplayState>()
        {
            new KitchenGameplayState(this, actor)
        };
    }

    public class KitchenGameplayState : AbstractGameplayState
    {
        
        public KitchenGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {
            
        }
        
        public override void Initialize()
        {

        }

        public override void Enter()
        {
            
        }
        public override void LogicUpdate()
        {
            
        }
        public override void PhysicsUpdate()
        {
            
        }
        public override void Interrupt()
        {
            
        }
        public override void Conclude()
        {
            // Base implementation transitions to relevant initial moderator state
            base.Conclude();
        }
        public override void Exit()
        {
            
        }

    }
}

