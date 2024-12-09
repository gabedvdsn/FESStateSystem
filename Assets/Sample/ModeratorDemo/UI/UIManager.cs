using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FESStateSystem.ModeratorDemo
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public TMP_Text ProgressText;
        public Slider ProgressSlider;

        [Space]
    
        public StateActor Actor;
        public TMP_Text WorldStateText;
        public StateContextTagScriptableObject LocationContext;
    
        [Space]
    
        public TMP_Text GameplayStateText;
        public StateContextTagScriptableObject EnergyContext;
    
        [Space]
    
        public TMP_Text CharacterStateText;
        public StateContextTagScriptableObject ActivityContext;
    
        [Space]
    
        public TMP_Text ModeratorText;
    
        private void Awake()
        {
            Instance = this;
        }

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

        private void Update()
        {
            if (Actor.Moderator is null) return;

            ModeratorText.text = Actor.Moderator.BaseModerator.name;

            if (Actor.Moderator.TryGetActiveState(LocationContext, out AbstractGameplayState worldState)) WorldStateText.text = $"Location: {worldState.StateData.name}";
            if (Actor.Moderator.TryGetActiveState(EnergyContext, out AbstractGameplayState gameplayState)) GameplayStateText.text = $"Energy: {gameplayState.StateData.name}";
            if (Actor.Moderator.TryGetActiveState(ActivityContext, out AbstractGameplayState characterState)) CharacterStateText.text = $"Activity: {characterState.StateData.name}";
        }
    }
}