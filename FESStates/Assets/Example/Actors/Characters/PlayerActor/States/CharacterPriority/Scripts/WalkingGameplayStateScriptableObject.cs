using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Walking State")]
public class WalkingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new WalkingGameplaySate(this, actor as PlayerStateActor);
    }
    
    public class WalkingGameplaySate : AbstractPlayerGameplayState
    {

        public WalkingGameplaySate(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
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
