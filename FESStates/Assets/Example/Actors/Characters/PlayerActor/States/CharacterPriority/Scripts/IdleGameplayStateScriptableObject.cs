using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Idle State")]
public class IdleGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{

    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new IdleGameplaySate(this, actor as PlayerStateActor);
    }

    public class IdleGameplaySate : AbstractPlayerGameplayState
    {

        public IdleGameplaySate(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
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
