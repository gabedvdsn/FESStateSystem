using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem.ModeratorDemo
{
    public class HomeGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
    {
    
        public override List<AbstractGameplayState> GenerateStates(StateActor actor)
        {
            return new List<AbstractGameplayState>()
            {
                new HomeGameplayState(this, actor)
            };
        }

        public class HomeGameplayState : AbstractPlayerGameplayState
        {
        
            public HomeGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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