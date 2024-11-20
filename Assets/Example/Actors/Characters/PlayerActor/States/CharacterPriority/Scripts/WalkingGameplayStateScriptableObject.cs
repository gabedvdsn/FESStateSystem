﻿using UnityEngine;

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