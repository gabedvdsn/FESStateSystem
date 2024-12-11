using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Group")]
    public class GameplayStateGroupScriptableObject : BaseAbstractGameplayStateScriptableObject
    {
        public List<BaseAbstractGameplayStateScriptableObject> States;
        
        public override List<AbstractGameplayState> GenerateStates(StateActor actor)
        {
            List<AbstractGameplayState> states = new List<AbstractGameplayState>();
            foreach (BaseAbstractGameplayStateScriptableObject baseState in States)
            {
                states.AddRange(baseState.GenerateStates(actor));
            }

            return states;
        }

        public override bool Defines(AbstractGameplayStateScriptableObject state)
        {
            return States.Contains(state);
        }

        public override List<AbstractGameplayStateScriptableObject> Get()
        {
            List<AbstractGameplayStateScriptableObject> states = new List<AbstractGameplayStateScriptableObject>();
            foreach (BaseAbstractGameplayStateScriptableObject baseState in States)
            {
                states.AddRange(baseState.Get());
            }

            return states;
        }
    }
}
