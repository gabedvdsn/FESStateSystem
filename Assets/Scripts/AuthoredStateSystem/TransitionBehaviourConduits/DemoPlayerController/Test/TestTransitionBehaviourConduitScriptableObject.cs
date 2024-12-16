using UnityEngine;
using FESStateSystem;
using FESStateSystem.TransitionBehaviourDemo;

[CreateAssetMenu(menuName = "FESState/Authored/Transition Behaviour Conduit/DemoPlayerController/Test", fileName = "TestConduit")]
public class TestTransitionBehaviourConduitScriptableObject : AbstractTransitionBehaviourConduitScriptableObject<DemoPlayerController>
{
    [Header("Conduit Prefab")]
    public TestTransitionBehaviourConduit ConduitPrefab;  // DO NOT change the name of this field
    
    protected override AbstractTransitionBehaviourConduit<DemoPlayerController> CreateConduit(AbstractTransitionBehaviourComponent<DemoPlayerController> transitionComponent)
    {
        if (!ConduitPrefab) return null;

        TestTransitionBehaviourConduit conduit = InstantiateConduit(ConduitPrefab, transitionComponent.transform);

        // Write additional logic

        return conduit;
    }
}

