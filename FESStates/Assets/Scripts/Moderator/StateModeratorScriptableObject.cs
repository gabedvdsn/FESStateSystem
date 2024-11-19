using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FESState/Moderator")]
public class StateModeratorScriptableObject : ScriptableObject
{
    [Header("Priority")]
    
    public StatePriorityTagScriptableObject ModeratorPriority;
    
    [Header("Initial States")]
    
    [SerializedDictionary("Priority Tag", "Initial State")]
    public SerializedDictionary<StatePriorityTagScriptableObject, AbstractGameplayStateScriptableObject> InitialStates;
    
    [Header("Defined Priority States")]
    
    [SerializedDictionary("Priority Tag", "States")]
    [SerializeField] public SerializedDictionary<StatePriorityTagScriptableObject, GameplayStateGroupScriptableObject> StatesByPriority;
    
    [Header("System Change Behaviours")]
    
    public List<AbstractSystemChangeResponseScriptableObject> SystemChangeBehaviours;

    public StateModerator GenerateModerator(StateActor actor) => new(this, actor);
    
}

public class StateModerator
{
    public StateModeratorScriptableObject BaseModerator;
    public StateActor Actor;

    private Dictionary<StatePriorityTagScriptableObject, GameplayStateMachine> PriorityStateMachines;
    private Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayState>> StoredStates;
    
    public StateModerator(StateModeratorScriptableObject moderator, StateActor actor)
    {
        BaseModerator = moderator;
        Actor = actor;
        
        InitializeModeratorStates();
    }

    private void InitializeModeratorStates()
    {
        StoredStates = new Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayState>>();
        PriorityStateMachines = new Dictionary<StatePriorityTagScriptableObject, GameplayStateMachine>();
        
        foreach (StatePriorityTagScriptableObject priorityTag in BaseModerator.InitialStates.Keys)
        {
            PriorityStateMachines[priorityTag] = new GameplayStateMachine();
            AbstractGameplayState initialState = BaseModerator.InitialStates[priorityTag].GenerateState(Actor);
            PriorityStateMachines[priorityTag].Initialize(initialState);
            
            StateIsChanged(priorityTag, null, initialState);

            StoredStates[priorityTag] = new List<AbstractGameplayState>();
            StoredStates[priorityTag].Add(initialState);
            foreach (AbstractGameplayStateScriptableObject state in BaseModerator.StatesByPriority[priorityTag].States)
            {
                if (state == initialState.StateData) continue;
                StoredStates[priorityTag].Add(state.GenerateState(Actor));
            }
        }
    }

    /// <summary>
    /// Changes the active state under some priority tag. Mutually exclusive from InterruptChangeState()
    /// </summary>
    /// <param name="priorityTag">The priority tag of the state to change.</param>
    /// <param name="newState">The state to change to.</param>
    /// <param name="onlyDefined">Disallow non-defined states.</param>
    public void DefaultChangeState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState, bool onlyDefined = true)
    {
        if (!DefinesState(priorityTag, newState) && onlyDefined) return;
        if (!TryGetStoredState(priorityTag, newState, out AbstractGameplayState state)) state = newState.GenerateState(Actor);
        
        StateIsChanged(priorityTag, PriorityStateMachines[priorityTag].CurrentState, state);
        PriorityStateMachines[priorityTag].ChangeState(state);
    }

    /// <summary>
    /// Interrupts and changes the active state under some priority tag. Mutually exclusive from DefaultChangeState()
    /// </summary>
    /// <param name="priorityTag">The priority tag of the state to change.</param>
    /// <param name="newState">The state to change to.</param>
    /// <param name="onlyDefined">Disallow non-defined states.</param>
    public void InterruptChangeState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState, bool onlyDefined = true)
    {
        if (!DefinesState(priorityTag, newState) && onlyDefined) return;
        if (!TryGetStoredState(priorityTag, newState, out AbstractGameplayState state))
        {
            state = newState.GenerateState(Actor);
            StoredStates[priorityTag].Add(state);
        }
        
        StateIsChanged(priorityTag, PriorityStateMachines[priorityTag].CurrentState, state);
        PriorityStateMachines[priorityTag].InterruptChangeState(state);
    }

    private void StateIsChanged(StatePriorityTagScriptableObject priorityTag, AbstractGameplayState oldState, AbstractGameplayState newState)
    {
        foreach (AbstractSystemChangeResponseScriptableObject stateChangeBehaviour in BaseModerator.SystemChangeBehaviours)
            stateChangeBehaviour.OnStateChanged(Actor, priorityTag, oldState, newState);
    }

    private void ModeratorIsChanged(StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
    {
        foreach (AbstractSystemChangeResponseScriptableObject moderatorChangeBehaviour in BaseModerator.SystemChangeBehaviours) moderatorChangeBehaviour.OnModeratorChanged(Actor, oldModerator, newModerator);
    }

    public void ReturnToInitial(AbstractGameplayStateScriptableObject sourceState)
    {
        foreach (StatePriorityTagScriptableObject priorityTag in StoredStates.Keys)
        {
            if (StoredStates[priorityTag].Any(state => state.StateData == sourceState))
            {
                DefaultChangeState(priorityTag, BaseModerator.InitialStates[priorityTag]);
                return;
            }
        }
        
        // Cannot find the sourceState, return all priorities to initial states (hard reset)
        foreach (StatePriorityTagScriptableObject priorityTag in PriorityStateMachines.Keys)
        {
            DefaultChangeState(priorityTag, BaseModerator.InitialStates[priorityTag]);
        }
    }

    public bool TryGetActiveStatePriority(AbstractGameplayStateScriptableObject state, out StatePriorityTagScriptableObject statePriorityTag)
    {
        statePriorityTag = null;
        foreach (StatePriorityTagScriptableObject priorityTag in StoredStates.Keys.Where(priorityTag => StoredStates[priorityTag].Any(storedState => storedState.StateData == state)))
        {
            statePriorityTag = priorityTag;
            return true;
        }

        return false;
    }

    public bool TryGetActiveState(StatePriorityTagScriptableObject priorityTag, out AbstractGameplayState state)
    {
        state = null;
        if (PriorityStateMachines.TryGetValue(priorityTag, out GameplayStateMachine stateMachine))
        {
            state = stateMachine.CurrentState;
            return true;
        }

        return false;
    }

    public bool TryGetStoredState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject sourceState, out AbstractGameplayState state)
    {
        state = null;
        foreach (AbstractGameplayState storedState in StoredStates[priorityTag].Where(storedState => storedState.StateData == sourceState))
        {
            state = storedState;
            return true;
        }

        return false;
    }

    public Dictionary<StatePriorityTagScriptableObject, AbstractGameplayState> GetActiveStates()
    {
        Dictionary<StatePriorityTagScriptableObject, AbstractGameplayState> activeStates = new Dictionary<StatePriorityTagScriptableObject, AbstractGameplayState>();
        foreach (StatePriorityTagScriptableObject priorityTag in PriorityStateMachines.Keys)
        {
            activeStates[priorityTag] = PriorityStateMachines[priorityTag].CurrentState;
        }

        return activeStates;
    }

    public void RunStatesLogicUpdate()
    {
        foreach(GameplayStateMachine stateMachine in PriorityStateMachines.Values) stateMachine.CurrentState.LogicUpdate();
    }

    public void RunStatesPhysicsUpdate()
    {
        foreach(GameplayStateMachine stateMachine in PriorityStateMachines.Values) stateMachine.CurrentState.PhysicsUpdate();
    }

    public AbstractGameplayStateScriptableObject GetInitialState(StatePriorityTagScriptableObject priorityTag) =>
        BaseModerator.InitialStates.TryGetValue(priorityTag, out AbstractGameplayStateScriptableObject state) ? state : null;
    
    public List<AbstractGameplayStateScriptableObject> GetStatesByPriority(StatePriorityTagScriptableObject priorityTag) => BaseModerator.StatesByPriority.TryGetValue(priorityTag, out GameplayStateGroupScriptableObject statesGroup) ? statesGroup.States : null;
    
    public bool DefinesState(AbstractGameplayStateScriptableObject state) => BaseModerator.StatesByPriority.Keys.Any(priorityTag => BaseModerator.StatesByPriority[priorityTag].States.Contains(state));
    public bool DefinesState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state) => BaseModerator.StatesByPriority.TryGetValue(priorityTag, out GameplayStateGroupScriptableObject statesGroup) && statesGroup.States.Contains(state);
    
    public static bool operator >(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority > other.BaseModerator.ModeratorPriority;
    public static bool operator <(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority < other.BaseModerator.ModeratorPriority;
    public static bool operator >=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority >= other.BaseModerator.ModeratorPriority;
    public static bool operator <=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority <= other.BaseModerator.ModeratorPriority;

    public void ImplementModeratorMeta(MetaStateModerator metaModerator, bool reEnterSameStates)
    {
        ModeratorIsChanged(BaseModerator, metaModerator.BaseModerator);
        BaseModerator = metaModerator.BaseModerator;
        
        foreach (StatePriorityTagScriptableObject priorityTag in metaModerator.InitialStates.Keys)
        {
            // Add state machines for new priority levels
            if (!PriorityStateMachines.ContainsKey(priorityTag))
            {
                PriorityStateMachines[priorityTag] = new GameplayStateMachine();
                PriorityStateMachines[priorityTag].Initialize(metaModerator.InitialStates[priorityTag].GenerateState(Actor));
            }
            // If the priority tag exists, handle changing state as necessary
            else
            {
                if (!reEnterSameStates && PriorityStateMachines[priorityTag].CurrentState.StateData == metaModerator.InitialStates[priorityTag]) continue;
                InterruptChangeState(priorityTag, metaModerator.InitialStates[priorityTag]);
            }

            // Store new states 
            foreach (AbstractGameplayStateScriptableObject sourceState in metaModerator.BaseModerator.StatesByPriority[priorityTag].States)
            {
                if (!TryGetStoredState(priorityTag, sourceState, out AbstractGameplayState _)) StoredStates[priorityTag].Add(sourceState.GenerateState(Actor));
            }
        }
        
        // Dump state machines for unused priority levels
        List<StatePriorityTagScriptableObject> machinesToRemove = PriorityStateMachines.Keys.Where(priorityTag => !BaseModerator.StatesByPriority.Keys.Contains(priorityTag)).ToList();
        foreach (StatePriorityTagScriptableObject priorityTag in machinesToRemove)
        {
            PriorityStateMachines.Remove(priorityTag);
            StoredStates.Remove(priorityTag);
        }

        // Dump unusable states
        foreach (StatePriorityTagScriptableObject priorityTag in StoredStates.Keys)
        {
            List<AbstractGameplayState> statesToRemove = new List<AbstractGameplayState>();
            foreach (AbstractGameplayState storedState in StoredStates[priorityTag])
            {
                if (!DefinesState(priorityTag, storedState.StateData)) statesToRemove.Add(storedState);
            }
            foreach (AbstractGameplayState state in statesToRemove) StoredStates[priorityTag].Remove(state);
        }
    }

    private void PrepareForMetaImplementation(MetaStateModerator metaModerator)
    {
        
        // dump unusable stored states
        // store new states
    }
    
    public static MetaStateModerator GenerateMeta(StateModeratorScriptableObject moderator)
    {
        return new MetaStateModerator(moderator);
    }
    
    public class MetaStateModerator
    {
        public StateModeratorScriptableObject BaseModerator;
        public Dictionary<StatePriorityTagScriptableObject, AbstractGameplayStateScriptableObject> InitialStates;
        
        public MetaStateModerator(StateModeratorScriptableObject baseModerator)
        {
            BaseModerator = baseModerator;
            
            InitialStates = new Dictionary<StatePriorityTagScriptableObject, AbstractGameplayStateScriptableObject>();
            foreach (StatePriorityTagScriptableObject priorityTag in BaseModerator.InitialStates.Keys)
            {
                InitialStates[priorityTag] = BaseModerator.InitialStates[priorityTag];
            }
        }
    
        public bool DefinesState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state) => BaseModerator.StatesByPriority.TryGetValue(priorityTag, out GameplayStateGroupScriptableObject statesGroup) && statesGroup.States.Contains(state);
        public void ChangeState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject newState)
        {
            InitialStates[priorityTag] = newState;
        }
        public void FillEmptyStates(StateModerator other)
        {
            foreach (StatePriorityTagScriptableObject priorityTag in other.PriorityStateMachines.Keys)
            {
                if (InitialStates.Keys.Contains(priorityTag)) continue;
                InitialStates[priorityTag] = other.PriorityStateMachines[priorityTag].CurrentState.StateData;
            }
        }
    }
}