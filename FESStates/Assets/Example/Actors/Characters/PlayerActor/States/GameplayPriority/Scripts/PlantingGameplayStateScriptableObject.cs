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
            base.Enter();
            base.Enter();
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Planting");
            // Play planting animation
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            UIManager.Instance.SetProgressSliderValue(progress);
            progress += speed * Time.deltaTime;
            if (progress >= 1f)
            {
                Conclude();
            }
            // Run planting progress bar
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // Nothing needed
        }
        public override void Interrupt()
        {
            base.Interrupt();
            UIManager.Instance.DisableProgressSlider();
            // Don't plant
            // Cancel planting progress bar
        }
        public override void Conclude()
        {
            base.Conclude();
            UIManager.Instance.DisableProgressSlider();
            Player.Moderator.ReturnToInitial(GameplayState);
        }
        public override void Exit()
        {
            base.Exit();
            // Plant plant
            // Conclude planting progress bar
            // Exit planting animation
        }
    }
}


