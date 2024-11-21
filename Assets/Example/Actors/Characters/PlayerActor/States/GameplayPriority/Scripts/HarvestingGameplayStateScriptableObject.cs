using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Harvesting State")]
public class HarvestingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override AbstractGameplayState GenerateState(StateActor actor)
    {
        return new HarvestingGameplayState(this, actor);
    }
    
    public class HarvestingGameplayState : AbstractPlayerGameplayState
    {
        private float progress;
        private float speed = .25f;
        public HarvestingGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {
        }
        public override void Initialize(StateActor actor)
        {
            
        }
        public override void Enter()
        {
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Harvesting");
            // Play harvesting animation
        }
        public override void LogicUpdate()
        {
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
            // Nothing needed
        }
        public override void Interrupt()
        {
            // Don't collect harvesting rewards
        }
        public override void Conclude()
        {
            // Collect harvesting rewards
            State.Moderator.ReturnToInitial(StateData);
        }
        public override void Exit()
        {
            UIManager.Instance.DisableProgressSlider();
        }
    }
}


