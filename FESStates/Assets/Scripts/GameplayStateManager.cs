using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

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
    [SerializeField] private StateTriggerScriptableObject DefaultStateTrigger;

    private List<StateActor> SubscribedActors = new();
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
    
    public void SubscribeActor(StateActor actor)
    {
        SubscribedActors.Add(actor);
        if (Initialized)
        {
            DistributeIndividualEnvironmentTrigger(actor, DefaultStateEnvironmentIdentifier);
        }
    }

    public void PerformInitialDistribution(GameplayStateTagScriptableObject environmentIdentifier)
    {
        DistributeInitialEnvironmentTriggers(environmentIdentifier);
        Initialized = true;
    }

    public void Reset()
    {
        Initialized = false;
        foreach (StateActor actor in SubscribedActors)
        {
            Destroy(actor.gameObject);
        }
    }

    private void DistributeInitialEnvironmentTriggers(GameplayStateTagScriptableObject environmentIdentifier)
    {
        StateEnvironmentScriptableObject environment = GetStateEnvironment(environmentIdentifier);
        foreach (StateActor actor in SubscribedActors)
        {
            InitializationStateTriggerScriptableObject initialTrigger = environment.GetInitialStateTrigger(actor.GeneralIdentifier);
            if (initialTrigger) initialTrigger.Activate(actor);
            else DefaultStateTrigger.Activate(actor);
        }
    }

    private void DistributeIndividualEnvironmentTrigger(StateActor actor, GameplayStateTagScriptableObject environmentIdentifier)
    {
        InitializationStateTriggerScriptableObject initialTrigger = GetStateEnvironment(environmentIdentifier).GetInitialStateTrigger(actor.GeneralIdentifier);
        if (initialTrigger) initialTrigger.Activate(actor);
        else DefaultStateTrigger.Activate(actor);
    }

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
