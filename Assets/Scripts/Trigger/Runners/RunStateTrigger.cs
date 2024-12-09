using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public class RunStateTrigger : AbstractTriggerRunner
    {
        public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
        public StateTriggerScriptableObject Trigger;

        public override void Run(bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveActor(out StateActor actor)) return;
            GameplayStateManager.Instance.RunDefaultTrigger(actor, Trigger, flag);
        }

        public override void Run(StateActor actor, bool flag = true) => GameplayStateManager.Instance.RunDefaultTrigger(actor, Trigger, flag);

        public override void RunMany(int count = -1, bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveManyActors(count, out List<StateActor> actors)) return;
            GameplayStateManager.Instance.RunDefaultManyTrigger(actors, Trigger, flag);
        }

        public override void RunMany(List<StateActor> actors, bool flag = true)
        {
            GameplayStateManager.Instance.RunDefaultManyTrigger(actors, Trigger, flag);
        }
        
        public override void RunAll(bool flag = true)
        {
            if (!ActorRetrieval.TryRetrieveManyActors(-1, out List<StateActor> actors)) return;
            GameplayStateManager.Instance.RunDefaultManyTrigger(actors, Trigger, flag);
        }
    }
}