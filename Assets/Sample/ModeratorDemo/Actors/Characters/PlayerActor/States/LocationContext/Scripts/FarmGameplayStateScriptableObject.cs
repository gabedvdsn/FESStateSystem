using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem.ModeratorDemo
{
    public class FarmGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
    {
    
        public override List<AbstractGameplayState> GenerateStates(StateActor actor)
        {
            return new List<AbstractGameplayState>()
            {
                new FarmGameplayState(this, actor)
            };
        }

        public class FarmGameplayState : AbstractPlayerGameplayState
        {
        
            public FarmGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
}