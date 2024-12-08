using System;
using System.Collections;
using System.Collections.Generic;
using FESStateSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text ProgressText;
    public Slider ProgressSlider;

    [Space]
    
    public StateActor Actor;
    public TMP_Text WorldStateText;
    public StatePriorityTagScriptableObject WorldPriority;
    
    [Space]
    
    public TMP_Text GameplayStateText;
    public StatePriorityTagScriptableObject GameplayPriority;
    
    [Space]
    
    public TMP_Text CharacterStateText;
    public StatePriorityTagScriptableObject CharacterPriority;
    
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

        if (Actor.Moderator.TryGetActiveState(WorldPriority, out AbstractGameplayState worldState)) WorldStateText.text = $"World: {worldState.StateData.name}";
        if (Actor.Moderator.TryGetActiveState(GameplayPriority, out AbstractGameplayState gameplayState)) GameplayStateText.text = $"Gameplay: {gameplayState.StateData.name}";
        if (Actor.Moderator.TryGetActiveState(CharacterPriority, out AbstractGameplayState characterState)) CharacterStateText.text = $"Character: {characterState.StateData.name}";
    }
}
