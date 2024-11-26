using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/General/Kleptao")]
public class KleptaoGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new KleptaoGameplayState(this, actor);
    }

    public class KleptaoGameplayState : AbstractGameplayState
    {
        public KleptaoGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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

