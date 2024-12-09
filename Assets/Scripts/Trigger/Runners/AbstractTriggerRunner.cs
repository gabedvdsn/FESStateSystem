using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractTriggerRunner : MonoBehaviour
    {
        public abstract void RunDefault(bool flag = true);
        public abstract void RunDefault(StateActor actor, bool flag = true);
        public abstract void RunDefaultMany(int count = -1, bool flag = true);
        public abstract void RunDefaultMany(List<StateActor> actors, bool flag = true);
        public abstract void RunDefaultAll(bool flag = true);
    }
}