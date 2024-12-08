using System.Collections;
using System.Collections.Generic;
using FESStateSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Cooking State")]
public class CookingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override List<AbstractGameplayState> GenerateState(StateActor actor)
    {
        return new List<AbstractGameplayState>()
        {
            new CookingGameplayState(this, actor)
        };
    }
    
    public class CookingGameplayState : AbstractPlayerGameplayState
    {
        private float progress;
        private float speed = .75f;
        
        public CookingGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {
        }

        public override void Enter()
        {
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Cooking");
            // Play cooking animation
        }
        public override void LogicUpdate()
        {
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
            // Nothing needed
        }
        
        public override void Interrupt()
        {
            // Don't collect cooking rewards
        }
        
        public override void Conclude()
        {
            // Collect cooking rewards
            
            base.Conclude();
        }
        public override void Exit()
        {
            UIManager.Instance.DisableProgressSlider();
            // Exit cooking animation
        }
    }
}

