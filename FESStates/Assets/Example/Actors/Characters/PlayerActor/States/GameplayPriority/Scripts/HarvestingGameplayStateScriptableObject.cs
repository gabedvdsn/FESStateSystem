using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Harvesting State")]
public class HarvestingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new HarvestingGameplayState(this, actor as PlayerStateActor);
    }
    
    public class HarvestingGameplayState : AbstractPlayerGameplayState
    {
        private float progress;
        private float speed = .25f;
        public HarvestingGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
        {
        }
        public override void Enter()
        {
            base.Enter();
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Harvesting");
            // Play harvesting animation
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
            // Run harvesting progress bar
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
            // Don't collect harvesting rewards
            // Cancel harvesting progress bar
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
            // Collect harvesting rewards
            // Conclude harvesting progress bar
            // Exit harvesting animation
        }
    }
}


