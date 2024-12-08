using System.Collections;
using System.Collections.Generic;
using FESStateSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Tired State")]
public class TiredGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new TiredGameplayState(this, actor);
    }
    
    public class TiredGameplayState : AbstractPlayerGameplayState
    {

        public TiredGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
