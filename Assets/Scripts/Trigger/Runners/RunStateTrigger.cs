using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public class RunStateTrigger : AbstractTriggerRunner
    {
        public AbstractRetrieveStateActorScriptableObject ActorRetrieval;
        public StateTriggerScriptableObject Trigger;

        public override void RunDefault()
        {
            GameplayStateManager.Instance.RunDefaultTrigger(ActorRetrieval, Trigger);
        }

        public override void RunDefaultMany(int count = -1)
        {
            GameplayStateManager.Instance.RunDefaultManyTrigger(ActorRetrieval, count, Trigger);
        }
        public override void RunDefaultAll()
        {
            GameplayStateManager.Instance.RunDefaultManyTrigger(ActorRetrieval, -1, Trigger);
        }

        public override void RunActorSpecific<T>()
        {
            GameplayStateManager.Instance.RunActorSpecificTrigger<T>(ActorRetrieval, Trigger);
        }

        public override void RunActorSpecificMany<T>(int count = -1)
        {
            GameplayStateManager.Instance.RunActorSpecificManyTrigger<T>(ActorRetrieval, count, Trigger);
        }
        public override void RunActorSpecificAll<T>()
        {
            GameplayStateManager.Instance.RunActorSpecificManyTrigger<T>(ActorRetrieval, -1, Trigger);
        }
    }
}