using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Test")]
public class TestPlayerGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new TestPlayerGameplayState(this, actor);
    }

    public class TestPlayerGameplayState : AbstractPlayerGameplayState
    {
        public TestPlayerGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
            
        }
        public override void Exit()
        {
            
        }

    }
}

