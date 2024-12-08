using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public abstract class AbstractTransitionPredicate<S>
    {
        protected S Source;
        
        protected AbstractTransitionPredicate(S source)
        {
            Source = source;
        }

        public abstract bool Evaluate();
    }
}
