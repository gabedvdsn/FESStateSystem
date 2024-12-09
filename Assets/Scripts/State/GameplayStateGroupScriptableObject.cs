using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Group")]
    public class GameplayStateGroupScriptableObject : AbstractGameplayStateBehaviourScriptableObject
    {
        public List<AbstractGameplayStateBehaviourScriptableObject> States;
        
        public override List<AbstractGameplayState> GenerateStates(StateActor actor)
        {
            List<AbstractGameplayState> states = new List<AbstractGameplayState>();
            foreach (AbstractGameplayStateBehaviourScriptableObject state in States)
            {
                states.AddRange(state.GenerateStates(actor));
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
            foreach (AbstractGameplayStateBehaviourScriptableObject stateBehaviour in States)
            {
                states.AddRange(stateBehaviour.Get());
            }

            return states;
        }
    }
}
