using UnityEngine;
using System.Collections.Generic;
using FESStateSystem;
using FESStateSystem.ModeratorDemo;

[CreateAssetMenu(menuName = "FESState/Authored/State/Player/Eating State", fileName = "EatingState")]
public class EatingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    
    public override List<AbstractGameplayState> GenerateStates(StateActor actor)
    {
        return new List<AbstractGameplayState>()
        {
            new EatingGameplayState(this, actor)
        };
    }

    public class EatingGameplayState : AbstractPlayerGameplayState
    {
        
        public EatingGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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

