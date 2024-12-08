using FESStateSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Walking State")]
public class WalkingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new WalkingGameplaySate(this, actor);
    }
    
    public class WalkingGameplaySate : AbstractPlayerGameplayState
    {

        public WalkingGameplaySate(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
