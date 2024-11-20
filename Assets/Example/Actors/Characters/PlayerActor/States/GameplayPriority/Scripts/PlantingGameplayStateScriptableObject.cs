using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Planting State")]
public class PlantingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new PlantingGameplayState(this, actor as PlayerStateActor);
    }
    
    public class PlantingGameplayState : AbstractPlayerGameplayState
    {
        private float progress;
        private float speed = .5f;
        public PlantingGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
        {
        }
        public override void Enter()
        {
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Planting");
            // Play planting animation
        }
        public override void LogicUpdate()
        {
            UIManager.Instance.SetProgressSliderValue(progress);
            progress += speed * Time.deltaTime;
            if (progress >= 1f)
            {
                Conclude();
            }
        }
        public override void PhysicsUpdate()
        {
            // Nothing needed
        }
        public override void Interrupt()
        {
            // Don't plant
        }
        public override void Conclude()
        {
            // Plant plant
            Player.Moderator.ReturnToInitial(StateData);
        }
        public override void Exit()
        {
            UIManager.Instance.DisableProgressSlider();
            // Exit planting animation
        }
    }
}


