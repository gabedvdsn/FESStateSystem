
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Free State")]
public class FreeGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new FreeGameplayState(this, actor as PlayerStateActor);
    }
    
    public class FreeGameplayState : AbstractPlayerGameplayState
    {

        public FreeGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
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
