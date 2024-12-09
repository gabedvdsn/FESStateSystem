using System.Collections;
using System.Collections.Generic;
using FESStateSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Planting State")]
public class PlantingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
{
    public override List<AbstractGameplayState> GenerateStates(StateActor actor)
    {
        return new List<AbstractGameplayState>()
        {
            new PlantingGameplayState(this, actor)
        };
    }

    public class PlantingGameplayState : AbstractPlayerGameplayState
    {
        private float progress;
        private float plantSpeed = .5f;
        
        public PlantingGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
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
            progress += plantSpeed * Time.deltaTime;
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
            base.Conclude();
        }
        public override void Exit()
        {
            UIManager.Instance.DisableProgressSlider();
            // Exit planting animation
        }
    }
}


