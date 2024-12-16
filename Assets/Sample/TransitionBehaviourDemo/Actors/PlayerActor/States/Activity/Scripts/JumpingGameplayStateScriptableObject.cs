using UnityEngine;
using System.Collections.Generic;
using FESStateSystem;
using FESStateSystem.ModeratorDemo;

[CreateAssetMenu(menuName = "FESState/Authored/State/Player/Jumping State", fileName = "JumpingState")]
public class JumpingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    
    public override List<AbstractGameplayState> GenerateStates(StateActor actor)
    {
        return new List<AbstractGameplayState>()
        {
            new JumpingGameplayState(this, actor)
        };
    }

    public class JumpingGameplayState : AbstractPlayerGameplayState
    {
        
        public JumpingGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
