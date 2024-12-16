using UnityEngine;
using FESStateSystem;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using FESStateSystem.TransitionBehaviourDemo;

public abstract class AbstractDemoPlayerControllerTransitionBehaviourConduit : AbstractTransitionBehaviourConduit<DemoPlayerController>
{
    [SerializedDictionary("Frequency", "Transition Data")]
    [SerializeField] private SerializedDictionary<DemoPlayerControllerFrequency, TransitionFrequencyData> TransitionFrequencies;

    private Dictionary<DemoPlayerControllerFrequency, LiveFrequencyTransitionData<DemoPlayerController>> LiveTransitionFrequencies;

    protected override void InitializeFrequencies()
    {
        LiveTransitionFrequencies = new Dictionary<DemoPlayerControllerFrequency, LiveFrequencyTransitionData<DemoPlayerController>>();

        foreach (DemoPlayerControllerFrequency frequency in TransitionFrequencies.Keys)
        {
            LiveTransitionFrequencies[frequency] = TransitionFrequencies[frequency].ToLiveData<DemoPlayerController>();
        }
    }

    public void Transmit(DemoPlayerControllerFrequency frequency)
    {
        TransmitOn(frequency);
    }
    
    protected bool TransmitOn(DemoPlayerControllerFrequency frequency)
    {
        if (!RecipientComponent) return false;
        return TryGetTransitionOn(frequency, out LiveFrequencyTransitionData<DemoPlayerController> frequencyData) && RecipientComponent.SendConduitTransition(this, frequencyData);
    }

    private bool TryGetTransitionOn(DemoPlayerControllerFrequency frequency, out LiveFrequencyTransitionData<DemoPlayerController> frequencyData)
    {
        return LiveTransitionFrequencies.TryGetValue(frequency, out frequencyData);
    }
    
  
}


public enum DemoPlayerControllerFrequency
{

}

