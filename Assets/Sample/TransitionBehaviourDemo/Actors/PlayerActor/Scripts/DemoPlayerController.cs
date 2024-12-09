using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem.TransitionBehaviourDemo
{
    public class DemoPlayerController : MonoBehaviour
    {
        public float Energy = 100f;

        public void ChangeEnergy(float delta)
        {
            Energy = Mathf.Clamp(Energy + delta, 0, 100);
        }
    }
}
