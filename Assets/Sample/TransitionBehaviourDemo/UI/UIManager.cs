using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FESStateSystem.TransitionBehaviourDemo
{
    public class UIManager : MonoBehaviour
    {
        public StateActor Actor;
        
        [Space]
        
        public Slider EnergySlider;
        public TMP_Text EnergyProgressText;

        [Space]
        
        public Slider ProgressSlider;
        public TMP_Text ProgressText;
        
        [Space]
        
        public TMP_Text ModeratorText;
        
        public TMP_Text EnergyText;
        public StateContextTagScriptableObject EnergyContext;
        
        public TMP_Text ActivityText;
        public StateContextTagScriptableObject ActivityContext;

        public void EnableProgressSlider(string progressText = "")
        {
            ProgressText.gameObject.SetActive(true);
            ProgressText.text = progressText;
        
            ProgressSlider.gameObject.SetActive(true);
            ProgressSlider.value = 0f;
        }

        public void DisableProgressSlider()
        {
            ProgressText.gameObject.SetActive(false);
            ProgressSlider.gameObject.SetActive(false);
        }

        public void SetProgressSliderValue(float value)
        {
            ProgressSlider.value = value;
        }
        
        public void SetEnergyProgressValue(float value)
        {
            ProgressSlider.value = value;
        }

        private void Update()
        {
            if (Actor.Moderator is null) return;

            ModeratorText.text = Actor.Moderator.BaseModerator.name;

            if (Actor.Moderator.TryGetActiveState(EnergyContext, out AbstractGameplayState gameplayState))
            {
                EnergyText.text = $"Energy: {gameplayState.StateData.name}";
                EnergyProgressText.text = $"Energy: {gameplayState.StateData.name}";
            }
            if (Actor.Moderator.TryGetActiveState(ActivityContext, out AbstractGameplayState characterState)) ActivityText.text = $"Activity: {characterState.StateData.name}";
        }
    }
}