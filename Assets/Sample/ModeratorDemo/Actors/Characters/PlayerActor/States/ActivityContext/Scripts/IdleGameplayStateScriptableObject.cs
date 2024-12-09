using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem.ModeratorDemo
{
    public class IdleGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
    {
        public override List<AbstractGameplayState> GenerateStates(StateActor actor)
        {
            return new List<AbstractGameplayState>()
            {
                new IdleGameplaySate(this, actor)
            };
        }

        public class IdleGameplaySate : AbstractPlayerGameplayState
        {

            public IdleGameplaySate(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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