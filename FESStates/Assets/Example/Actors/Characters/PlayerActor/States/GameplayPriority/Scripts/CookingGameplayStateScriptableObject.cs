using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Cooking State")]
public class CookingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new CookingGameplayState(this, actor as PlayerStateActor);
    }
    
    public class CookingGameplayState : AbstractPlayerGameplayState
    {
        private float progress;
        private float speed = .75f;
        
        public CookingGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
        {
        }
        public override void Enter()
        {
            base.Enter();
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Cooking");
            // Play cooking animation
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
            // Run cooking progress bar
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // Nothing needed
        }
        
        /// <summary>
        /// Does not transition state
        /// </summary>
        public override void Interrupt()
        {
            base.Interrupt();
            UIManager.Instance.DisableProgressSlider();
            // Don't collect cooking rewards
            // Cancel cooking progress bar
        }
        
        /// <summary>
        /// Natural state conclusion.
        /// Initiates state transition
        /// </summary>
        public override void Conclude()
        {
            base.Conclude();
            UIManager.Instance.DisableProgressSlider();
            Player.Moderator.ReturnToInitial(GameplayState);
        }
        public override void Exit()
        {
            base.Exit();
            
            // Collect cooking rewards
            // Conclude cooking progress bar
            // Exit cooking animation
        }
    }
}

