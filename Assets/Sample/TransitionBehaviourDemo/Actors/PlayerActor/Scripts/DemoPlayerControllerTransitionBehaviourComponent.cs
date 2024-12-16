using UnityEngine;
using FESStateSystem;
using FESStateSystem.TransitionBehaviourDemo;

/// <summary>
/// A transition behaviour component that interfaces with DemoPlayerController.
/// </summary>
[RequireComponent(typeof(DemoPlayerController))]
public class DemoPlayerControllerTransitionBehaviourComponent : AbstractTransitionBehaviourComponent<DemoPlayerController>
{
    /*
    Override options:
        OnAwakeEvent
        OnStartEvent
        
    Methods of interest:
        Run(StateTransition<DemoPlayerController> transition,
            StateContextTagScriptableObject contextTag,
            bool interruptStateChange = true
        )
        RunAll(bool interruptStateChange = true)
        RunWithinContext(StateContextTagScriptableObject contextTag,
            bool interruptStateChange = true
        )
    */

    // Implement behaviour as needed
}

