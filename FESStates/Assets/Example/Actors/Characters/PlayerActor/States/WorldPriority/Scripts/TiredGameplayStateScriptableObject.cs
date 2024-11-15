using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Tired State")]
public class TiredGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new TiredGameplayState(this, actor as PlayerStateActor);
    }
    
    public class TiredGameplayState : AbstractPlayerGameplayState
    {

        public TiredGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
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
