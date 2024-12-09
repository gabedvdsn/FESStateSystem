using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem.ModeratorDemo
{
    public class ExhaustedGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
    {
        public override List<AbstractGameplayState> GenerateStates(StateActor actor)
        {
            return new List<AbstractGameplayState>()
            {
                new ExhaustedGameplayState(this, actor)
            };
        }
    
        public class ExhaustedGameplayState : AbstractPlayerGameplayState
        {

            public ExhaustedGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
                base.Conclude();
            }
            public override void Exit()
            {
            
            }
        }
    }
}