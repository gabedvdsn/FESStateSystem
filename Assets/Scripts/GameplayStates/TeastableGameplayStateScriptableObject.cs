using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/General/Teastable")]
public class TeastableGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new TeastableGameplayState(this, actor);
    }

    public class TeastableGameplayState : AbstractGameplayState
    {
        public TeastableGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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

