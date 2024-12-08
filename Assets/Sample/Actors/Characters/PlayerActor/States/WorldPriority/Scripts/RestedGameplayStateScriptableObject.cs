using System.Collections;
using System.Collections.Generic;
using FESStateSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Rested State")]
public class RestedGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new SafeGameplayState(this, actor);
    }
    
    public class SafeGameplayState : AbstractPlayerGameplayState
    {

        public SafeGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
