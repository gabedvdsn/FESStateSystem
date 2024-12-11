using UnityEngine;

namespace FESStateSystem
{
    public interface ITransitionBehaviourConduit
    {
        public void ForceTransmitAll();
        public void CleanDependencies();
    }
}
