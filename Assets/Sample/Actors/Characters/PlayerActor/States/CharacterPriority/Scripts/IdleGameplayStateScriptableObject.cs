using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Idle State")]
public class IdleGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{

    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new IdleGameplaySate(this, actor);
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
            
        }
        public override void Exit()
        {
            
        }
    }
}
