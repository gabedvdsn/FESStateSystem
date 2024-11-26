using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/General/Latpka")]
public class LatpkaGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new LatpkaGameplayState(this, actor);
    }

    public class LatpkaGameplayState : AbstractGameplayState
    {
        public LatpkaGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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

