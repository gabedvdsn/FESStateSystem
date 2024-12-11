using UnityEngine;
using FESStateSystem;
using FESStateSystem.TransitionBehaviourDemo;

[CreateAssetMenu(menuName = "FESState/Authored/Transition Behaviour Conduit/DemoPlayerController/PlayerInput", fileName = "PlayerInputConduit")]
public class PlayerInputTransitionBehaviourConduitScriptableObject : AbstractTransitionBehaviourConduitScriptableObject<DemoPlayerController>
{
    [Header("Conduit Prefab")]
    public PlayerInputTransitionBehaviourConduit ConduitPrefab;
    
    protected override AbstractTransitionBehaviourConduit<DemoPlayerController> CreateConduit(AbstractTransitionBehaviourComponent<DemoPlayerController> transitionComponent)
    {
        if (!ConduitPrefab) return null;

        PlayerInputTransitionBehaviourConduit conduit = InstantiateConduit(ConduitPrefab);

        // Write additional logic

        return conduit;
    }
}

