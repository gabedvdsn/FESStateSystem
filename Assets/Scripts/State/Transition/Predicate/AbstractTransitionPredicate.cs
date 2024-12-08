using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractTransitionPredicate<S>
    {
        public abstract bool Evaluate(S source);
    }
}
