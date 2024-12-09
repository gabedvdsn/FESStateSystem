using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractTriggerRunner : MonoBehaviour
    {
        public abstract void Run(bool flag = true);
        public abstract void Run(StateActor actor, bool flag = true);
        public abstract void RunMany(int count = -1, bool flag = true);
        public abstract void RunMany(List<StateActor> actors, bool flag = true);
        public abstract void RunAll(bool flag = true);
    }
}