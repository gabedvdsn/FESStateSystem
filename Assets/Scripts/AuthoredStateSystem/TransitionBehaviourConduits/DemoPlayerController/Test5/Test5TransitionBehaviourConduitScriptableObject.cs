using UnityEngine;
using FESStateSystem;
using FESStateSystem.TransitionBehaviourDemo;

[CreateAssetMenu(menuName = "FESState/Authored/DemoPlayerController/Transition Behaviour Conduit/Test5", fileName = "Test5Conduit")]
public class Test5TransitionBehaviourConduitScriptableObject : AbstractTransitionBehaviourConduitScriptableObject<DemoPlayerController>
{
    [Header("Conduit Prefab")]
    public Test5TransitionBehaviourConduit ConduitPrefab;  // DO NOT change the name of this field
    
    protected override AbstractTransitionBehaviourConduit<DemoPlayerController> CreateConduit(AbstractTransitionBehaviourComponent<DemoPlayerController> transitionComponent)
    {
        if (!ConduitPrefab) return null;

        Test5TransitionBehaviourConduit conduit = InstantiateConduit(ConduitPrefab, transitionComponent.transform);

        // Write additional logic

        return conduit;
    }
}

