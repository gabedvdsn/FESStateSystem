using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/Trigger/Conditional Trigger Group", fileName = "ConditionalTriggerGroup")]
    public class ConditionalStateGroupTriggerScriptableObject : AbstractStateConditionalTriggerScriptableObject
    {
        public List<AbstractStateConditionalTriggerScriptableObject> Conditionals;
    
        public override bool Activate(StateActor actor, bool flag)
        {
            return Conditionals.All(c => c.Activate(actor, flag));
        }

        public override bool PreStateChangeActivate(StateActor actor, StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState)
        {
            // The new newState should align with one of the conditionals
            // The other conditionals should hold true for active/stored states (depending on conditional)
            return Conditionals.All(c => c.PreStateChangeActivate(actor, contextTag, newState));
        }

        public override Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> GetStates()
        {
            Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> states =
                new Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>>();
            
            foreach (AbstractStateConditionalTriggerScriptableObject conditional in Conditionals)
            {
                Dictionary<StateContextTagScriptableObject, List<AbstractGameplayStateScriptableObject>> cStates = conditional.GetStates();
                foreach (StateContextTagScriptableObject contextTag in cStates.Keys)
                {
                    if (states.ContainsKey(contextTag))
                    {
                        foreach (AbstractGameplayStateScriptableObject state in cStates[contextTag].Where(state => !states[contextTag].Contains(state)))
                        {
                            states[contextTag].Add(state);
                        }
                    }
                    else states[contextTag] = cStates[contextTag];
                }
            }

            return states;
        }

        public override bool PreModeratorChangeActivate(StateModeratorScriptableObject moderator)
        {
            return Conditionals.All(c => c.PreModeratorChangeActivate(moderator));
        }
    }
}