using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractTransitionBehaviourConduitScriptableObject<S> : ScriptableObject where S : MonoBehaviour
    {
        [Header("Abstract Conduit")]
        public bool ConduitStartsOpen = true;
        
        public void InitializeConduit(AbstractTransitionBehaviourComponent<S> transitionComponent)
        {
            AbstractTransitionBehaviourConduit<S> conduit = CreateConduit(transitionComponent);
            if (!conduit) return;
            
            // conduit.Initialize(transitionComponent, ConduitStartsOpen);
        }

        protected T InstantiateConduit<T>(T prefab, Transform parent) where T : AbstractTransitionBehaviourConduit<S>
        { 
            T conduit = Instantiate(prefab, parent);
            conduit.name = $"{typeof(T).Name.Split('`')[0].Replace("TransitionBehaviour", "")} [{typeof(S).Name.Split('`')[0]}]";
            return conduit;
        }
        
        protected abstract AbstractTransitionBehaviourConduit<S> CreateConduit(AbstractTransitionBehaviourComponent<S> transitionComponent);
    }
}
