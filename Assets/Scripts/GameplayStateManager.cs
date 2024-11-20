using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
    
    public GameplayStateTagScriptableObject DefaultStateEnvironmentIdentifier;
    
    [Space]

    [SerializedDictionary("State Environment Identifier", "State Environment")]
    [SerializeField] private SerializedDictionary<GameplayStateTagScriptableObject, StateEnvironmentScriptableObject> StateEnvironments;
    [SerializeField] private InitializationStateTriggerScriptableObject FallbackInitializationTrigger;

    private Dictionary<GameplayStateTagScriptableObject, List<StateActor>> SubscribedActors = new();
    public Dictionary<GameplayStateTagScriptableObject, List<StateActor>> AllActors => SubscribedActors;
    
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

    private void Start()
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
            DistributeIndividualEnvironmentTrigger(actor, DefaultStateEnvironmentIdentifier);
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

    public List<T> RetrieveActorsByTag<T>(GameplayStateTagScriptableObject actorTag) where T : StateActor
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
    
    public void PerformInitialDistribution(GameplayStateTagScriptableObject environmentIdentifier)
    {
        DistributeInitialEnvironmentTriggers(environmentIdentifier);
        Initialized = true;
    }

    public void Reset()
    {
        Initialized = false;
        foreach (GameplayStateTagScriptableObject actorTag in SubscribedActors.Keys)
        {
            foreach (StateActor actor in SubscribedActors[actorTag]) Destroy(actor.gameObject);
        }

        SubscribedActors.Clear();
    }

    private void DistributeInitialEnvironmentTriggers(GameplayStateTagScriptableObject environmentIdentifier)
    {
        StateEnvironmentScriptableObject environment = GetStateEnvironment(environmentIdentifier);
        foreach (GameplayStateTagScriptableObject actorTag in SubscribedActors.Keys)
        {
            foreach (StateActor actor in SubscribedActors[actorTag])
            {
                InitializationStateTriggerScriptableObject initialTrigger = environment.GetInitialStateTrigger(actor.GeneralIdentifier);
                if (initialTrigger) initialTrigger.Activate(actor);
                else FallbackInitializationTrigger.Activate(actor);
            }
        }
    }

    private void DistributeIndividualEnvironmentTrigger(StateActor actor, GameplayStateTagScriptableObject environmentIdentifier)
    {
        InitializationStateTriggerScriptableObject initialTrigger = GetStateEnvironment(environmentIdentifier).GetInitialStateTrigger(actor.GeneralIdentifier);
        if (initialTrigger) initialTrigger.Activate(actor);
        else FallbackInitializationTrigger.Activate(actor);
    }
    
    #endregion
    
    #region Trigger Runners

    public void RunDefaultTrigger(AbstractRetrieveStateActorScriptableObject actorRetrieval, AbstractStateTriggerScriptableObject trigger)
    {
        if (!actorRetrieval.TryRetrieveActor(out StateActor actor)) return;
        
        trigger.Activate(actor);
    }

    public void RunDefaultManyTrigger(AbstractRetrieveStateActorScriptableObject actorRetrieval, int count, AbstractStateTriggerScriptableObject trigger)
    {
        if (!actorRetrieval.TryRetrieveManyActors(count, out List<StateActor> actors)) return;
        
        foreach (StateActor actor in actors) trigger.Activate(actor);
    }
    
    public void RunActorSpecificTrigger<T>(AbstractRetrieveStateActorScriptableObject actorRetrieval, AbstractStateTriggerScriptableObject trigger) where T : StateActor
    {
        if (!actorRetrieval.TryRetrieveActor(out T actor)) return;

        trigger.Activate(actor);
    }

    public void RunActorSpecificManyTrigger<T>(AbstractRetrieveStateActorScriptableObject actorRetrieval, int count, AbstractStateTriggerScriptableObject trigger) where T : StateActor
    {
        if (!actorRetrieval.TryRetrieveManyActors(count, out List<T> actors)) return;

        foreach (T actor in actors) trigger.Activate(actor);
    }

    public void RunDefaultConditionalTrigger(AbstractRetrieveStateActorScriptableObject actorRetrieval, AbstractStateConditionalTriggerScriptableObject conditionalTrigger, UnityEvent<StateActor> onTrueEvent, UnityEvent<StateActor> onFalseEvent)
    {
        if (!actorRetrieval.TryRetrieveActor(out StateActor actor))
        {
            if (conditionalTrigger.FalseOnNullActor) onFalseEvent?.Invoke(actor);
            return;
        }

        if (conditionalTrigger.Activate(actor)) onTrueEvent?.Invoke(actor);
        else onFalseEvent?.Invoke(actor);
    }

    public void RunDefaultManyConditionalTrigger(AbstractRetrieveStateActorScriptableObject actorRetrieval, int count, AbstractStateConditionalTriggerScriptableObject conditionalTrigger, UnityEvent<StateActor> onTrueEvent, UnityEvent<StateActor> onFalseEvent)
    {
        if (!actorRetrieval.TryRetrieveManyActors(count, out List<StateActor> actors))
        {
            if (conditionalTrigger.FalseOnNullActor) onFalseEvent?.Invoke(null);
            return;
        }
        
        foreach (StateActor actor in actors)
        {
            if (conditionalTrigger.Activate(actor)) onTrueEvent?.Invoke(actor);
            else onFalseEvent?.Invoke(actor);
        }
    }
    
    public void RunActorSpecificConditionalTrigger<T>(AbstractRetrieveStateActorScriptableObject actorRetrieval, AbstractStateConditionalTriggerScriptableObject conditionalTrigger, UnityEvent<StateActor> onTrueEvent, UnityEvent<StateActor> onFalseEvent) where T : StateActor
    {
        if (!actorRetrieval.TryRetrieveActor(out T actor))
        {
            if (conditionalTrigger.FalseOnNullActor) onFalseEvent?.Invoke(null);
            return;
        }
        
        if (conditionalTrigger.Activate(actor)) onTrueEvent?.Invoke(actor);
        else onFalseEvent?.Invoke(actor);
    }
    public void RunActorSpecificConditionalManyTrigger<T>(AbstractRetrieveStateActorScriptableObject actorRetrieval, int count, AbstractStateConditionalTriggerScriptableObject conditionalTrigger, UnityEvent<StateActor> onTrueEvent, UnityEvent<StateActor> onFalseEvent) where T : StateActor
    {
        if (!actorRetrieval.TryRetrieveManyActors(count, out List<T> actors))
        {
            if (conditionalTrigger.FalseOnNullActor) onFalseEvent?.Invoke(null);
            return;
        }

        foreach (T actor in actors)
        {
            if (conditionalTrigger.Activate(actor)) onTrueEvent?.Invoke(actor);
            else onFalseEvent?.Invoke(actor);
        }
    }
    
    #endregion

    public StateEnvironmentScriptableObject GetStateEnvironment(GameplayStateTagScriptableObject StateEnvironmentIdentifier)
    {
        return StateEnvironments.TryGetValue(StateEnvironmentIdentifier, out StateEnvironmentScriptableObject stateEnvironment) ? stateEnvironment : StateEnvironments[DefaultStateEnvironmentIdentifier];
    }

    private void OnValidate()
    {
        if (DefaultStateEnvironmentIdentifier)
        {
            if (!StateEnvironments.Keys.Contains(DefaultStateEnvironmentIdentifier)) throw new Exception("Default State Environment Identifier must exist in State Environments");
        }
    }
}
