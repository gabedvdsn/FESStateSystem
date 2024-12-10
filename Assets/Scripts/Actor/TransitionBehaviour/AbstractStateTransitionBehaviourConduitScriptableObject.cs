using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractStateTransitionBehaviourConduitScriptableObject<S> : ScriptableObject where S : MonoBehaviour
    {
        [Header("Abstract Conduit")]
        public bool ConduitStartsOpen;
        
        public void InitializeConduit(AbstractStateTransitionBehaviourComponent<S> transitionComponent)
        {
            AbstractStateTransitionBehaviourConduit<S> conduit = InstantiateConduit(transitionComponent);
            conduit.Initialize(transitionComponent, ConduitStartsOpen);
        }
        
        protected abstract AbstractStateTransitionBehaviourConduit<S> InstantiateConduit(AbstractStateTransitionBehaviourComponent<S> transitionComponent);
    }
}
