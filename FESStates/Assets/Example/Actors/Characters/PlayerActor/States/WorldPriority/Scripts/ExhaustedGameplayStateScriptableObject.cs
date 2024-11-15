using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Exhausted State")]
public class ExhaustedGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new ExhaustedGameplayState(this, actor as PlayerStateActor);
    }
    
    public class ExhaustedGameplayState : AbstractPlayerGameplayState
    {

        public ExhaustedGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        public override void Interrupt()
        {
            base.Interrupt();
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
