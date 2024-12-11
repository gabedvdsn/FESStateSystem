using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace FESStateSystem
{
    public class GameplayStateManager : MonoBehaviour
    {
        private static GameplayStateManager instance;

        public static GameplayStateManager Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = FindObjectOfType<GameplayStateManager>();
                }

                return instance;
            }
        }
    
        public StateIdentifierTagScriptableObject DefaultStateEnvironmentIdentifier;
    
        [Space]

        [SerializedDictionary("State Environment Identifier", "State Environment")]
        [SerializeField] private SerializedDictionary<StateIdentifierTagScriptableObject, StateEnvironmentScriptableObject> StateEnvironments;
        [SerializeField] private InitializationStateTriggerScriptableObject FallbackInitializationTrigger;

        private Dictionary<StateIdentifierTagScriptableObject, List<StateActor>> SubscribedActors = new();
        public Dictionary<StateIdentifierTagScriptableObject, List<StateActor>> AllActors => SubscribedActors;
        private List<ITransitionBehaviourConduit> ActiveConduits;
        
        private bool Initialized;

        private void Awake()
        {
            if (instance != null && instance != this) Destroy(instance.gameObject);
            else
            {
                instance = this;    
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable()
        {
            PerformInitialDistribution(DefaultStateEnvironmentIdentifier);
        }
    
        #region Actors
    
        public void SubscribeActor(StateActor actor)
        {
            if (SubscribedActors.ContainsKey(actor.GeneralIdentifier)) SubscribedActors[actor.GeneralIdentifier].Add(actor);
            else SubscribedActors[actor.GeneralIdentifier] = new List<StateActor> { actor };
        
            if (Initialized)
            {
                DistributeEnvironmentInitializationTrigger(actor, DefaultStateEnvironmentIdentifier);
            }
        }

        public void UnsubscribeActor(StateActor actor)
        {
            if (SubscribedActors.ContainsKey(actor.GeneralIdentifier) && SubscribedActors[actor.GeneralIdentifier].Contains(actor))
            {
                SubscribedActors[actor.GeneralIdentifier].Remove(actor);
            }

            if (SubscribedActors[actor.GeneralIdentifier].Count == 0) SubscribedActors.Remove(actor.GeneralIdentifier);
        }

        public List<T> RetrieveActorsByTag<T>(StateIdentifierTagScriptableObject actorTag) where T : StateActor
        {
            if (!SubscribedActors.ContainsKey(actorTag)) return null;
        
            List<T> actors = new List<T>();
            foreach (StateActor actor in SubscribedActors[actorTag])
            {
                actors.Add(actor as T);
            }

            return actors;
        }
    
        #endregion
    
        #region Subscription Trigger Distribution
    
        public void PerformInitialDistribution(StateIdentifierTagScriptableObject environmentIdentifier)
        {
            DistributeAllEnvironmentInitializationTriggers(environmentIdentifier);
            Initialized = true;
        }

        
        
        private StateEnvironmentScriptableObject GetStateEnvironment(StateIdentifierTagScriptableObject StateEnvironmentIdentifier)
        {
            return StateEnvironments.TryGetValue(StateEnvironmentIdentifier, out StateEnvironmentScriptableObject stateEnvironment) ? stateEnvironment : StateEnvironments[DefaultStateEnvironmentIdentifier];
        }

        private void DistributeAllEnvironmentInitializationTriggers(StateIdentifierTagScriptableObject environmentIdentifier, bool flag = true)
        {
            StateEnvironmentScriptableObject environment = GetStateEnvironment(environmentIdentifier);
            foreach (StateIdentifierTagScriptableObject actorTag in SubscribedActors.Keys)
            {
                foreach (StateActor actor in SubscribedActors[actorTag])
                {
                    InitializationStateTriggerScriptableObject initialTrigger = environment.GetInitialStateTrigger(actor.GeneralIdentifier);
                    if (initialTrigger) initialTrigger.Activate(actor, flag);
                    else FallbackInitializationTrigger.Activate(actor, flag);
                }
            }
        }

        private void DistributeEnvironmentInitializationTrigger(StateActor actor, StateIdentifierTagScriptableObject environmentIdentifier, bool flag = true)
        {
            InitializationStateTriggerScriptableObject initialTrigger = GetStateEnvironment(environmentIdentifier).GetInitialStateTrigger(actor.GeneralIdentifier);
            if (initialTrigger) initialTrigger.Activate(actor, flag);
            else FallbackInitializationTrigger.Activate(actor, flag);
        }
    
        #endregion
    
        #region Trigger Runners

        public void RunDefaultTrigger(StateActor actor, AbstractStateTriggerScriptableObject trigger, bool flag = true)
        {
            trigger.Activate(actor, flag);
        }

        public void RunDefaultManyTrigger(List<StateActor> actors, AbstractStateTriggerScriptableObject trigger, bool flag = true)
        {
            foreach (StateActor actor in actors) trigger.Activate(actor, flag);
        }
        
        public void RunDefaultConditionalTrigger(StateActor actor, AbstractStateConditionalTriggerScriptableObject conditionalTrigger, PostConditionalEvent onTrueEvent, PostConditionalEvent onFalseEvent, bool flag = true)
        {
            if (conditionalTrigger.Activate(actor, flag)) onTrueEvent?.Run(actor, flag);
            else onFalseEvent?.Event?.Invoke(actor);
        }

        public void RunDefaultManyConditionalTrigger(List<StateActor> actors, AbstractStateConditionalTriggerScriptableObject conditionalTrigger, PostConditionalEvent onTrueEvent, PostConditionalEvent onFalseEvent, bool flag = true)
        {
            foreach (StateActor actor in actors)
            {
                if (conditionalTrigger.Activate(actor, flag)) onTrueEvent?.Run(actor, flag);
                else onFalseEvent?.Run(actor, flag);
            }
        }
        
        #endregion
        
        #region Conduits

        public bool ConduitIsRegistered(ITransitionBehaviourConduit conduit)
        {
            return ActiveConduits.Contains(conduit);
        }

        public void RegisterConduit(ITransitionBehaviourConduit conduit)
        {
            ActiveConduits ??= new List<ITransitionBehaviourConduit>();
            ActiveConduits.Add(conduit);
        }
        
        #endregion

        private void OnDisable()
        {
            if (ActiveConduits is null) return;
            foreach (ITransitionBehaviourConduit conduit in ActiveConduits)
            {
                conduit.CleanDependencies();
            }
        }

        private void OnValidate()
        {
            if (DefaultStateEnvironmentIdentifier)
            {
                if (!StateEnvironments.Keys.Contains(DefaultStateEnvironmentIdentifier)) throw new Exception("Default State Environment Identifier must exist in State Environments");
            }
        }
    }
}