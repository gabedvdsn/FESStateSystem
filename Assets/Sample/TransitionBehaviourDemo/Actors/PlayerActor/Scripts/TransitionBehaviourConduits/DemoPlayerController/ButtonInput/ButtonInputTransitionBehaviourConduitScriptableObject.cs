using UnityEngine;
using FESStateSystem;
using FESStateSystem.TransitionBehaviourDemo;

[CreateAssetMenu(menuName = "FESState/Authored/Transition Behaviour Conduit/DemoPlayerController/ButtonInput", fileName = "ButtonInputConduit")]
public class ButtonInputTransitionBehaviourConduitScriptableObject : AbstractTransitionBehaviourConduitScriptableObject<DemoPlayerController>
{
    [Header("Conduit Prefab")]
    public ButtonInputTransitionBehaviourConduit ConduitPrefab;
    
    protected override AbstractTransitionBehaviourConduit<DemoPlayerController> CreateConduit(AbstractTransitionBehaviourComponent<DemoPlayerController> transitionComponent)
    {
        if (!ConduitPrefab) return null;

        ButtonInputTransitionBehaviourConduit conduit = InstantiateConduit(ConduitPrefab);

        // Write additional logic

        return conduit;
    }
}

