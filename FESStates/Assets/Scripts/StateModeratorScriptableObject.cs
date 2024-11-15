using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Moderator")]
public class StateModeratorScriptableObject : ScriptableObject
{
    public StatePriorityTag ModeratorPriority;
    [SerializedDictionary("Priority Tag", "Initial State")]
    public SerializedDictionary<StatePriorityTag, AbstractGameplayStateScriptableObject> InitialStates;
    
    [Space]
    
    public bool BlockUndefinedTransitions = true;
    [SerializedDictionary("Priority Tag", "States")]
    [SerializeField] public SerializedDictionary<StatePriorityTag, List<AbstractGameplayStateScriptableObject>> StatesByPriority;

    public StateModerator GenerateModerator(StateActor actor) => new(this, actor);
    
}

public class StateModerator
{
    public StateModeratorScriptableObject BaseModerator;
    public StateActor Actor;

    private Dictionary<StatePriorityTag, GameplayStateMachine> PriorityStateMachines;
    private Dictionary<StatePriorityTag, List<AbstractGameplayState>> StoredStates;
    
    public StateModerator(StateModeratorScriptableObject moderator, StateActor actor)
    {
        BaseModerator = moderator;
        Actor = actor;
        
        StoredStates = new Dictionary<StatePriorityTag, List<AbstractGameplayState>>();
        PriorityStateMachines = new Dictionary<StatePriorityTag, GameplayStateMachine>();
        foreach (StatePriorityTag priorityTag in BaseModerator.InitialStates.Keys)
        {
            PriorityStateMachines[priorityTag] = new GameplayStateMachine();
            AbstractGameplayState initialState = BaseModerator.InitialStates[priorityTag].GenerateState(Actor);
            PriorityStateMachines[priorityTag].Initialize(initialState);

            StoredStates[priorityTag] = new List<AbstractGameplayState>();
            StoredStates[priorityTag].Add(initialState);
            foreach (AbstractGameplayStateScriptableObject storedState in BaseModerator.StatesByPriority[priorityTag])
            {
                if (storedState == initialState.GameplayState) continue;
                StoredStates[priorityTag].Add(storedState.GenerateState(actor));
            }
        }
    }

    public void ChangeState(StatePriorityTag priorityTag, AbstractGameplayStateScriptableObject newState, bool onlyDefined = true)
    {
        if (!DefinesState(priorityTag, newState) && onlyDefined) return;
        if (!TryGetStoredState(priorityTag, newState, out AbstractGameplayState state)) state = newState.GenerateState(Actor);
        PriorityStateMachines[priorityTag].ChangeState(state);
    }

    public void InterruptChangeState(StatePriorityTag priorityTag, AbstractGameplayStateScriptableObject newState, bool onlyDefined = true)
    {
        if (!DefinesState(priorityTag, newState) && onlyDefined) return;
        if (!TryGetStoredState(priorityTag, newState, out AbstractGameplayState state)) state = newState.GenerateState(Actor);
        PriorityStateMachines[priorityTag].InterruptChangeState(state);
    }

    public void ReturnToInitial(AbstractGameplayStateScriptableObject sourceState)
    {
        foreach (StatePriorityTag priorityTag in StoredStates.Keys)
        {
            if (StoredStates[priorityTag].Any(state => state.GameplayState == sourceState))
            {
                ChangeState(priorityTag, BaseModerator.InitialStates[priorityTag]);
                return;
            }
        }
        
        // Cannot find the sourceState, return all priorities to initial states (hard reset)
        foreach (StatePriorityTag priorityTag in PriorityStateMachines.Keys)
        {
            ChangeState(priorityTag, BaseModerator.InitialStates[priorityTag]);
        }
        
    }

    public bool TryGetActiveState(StatePriorityTag priorityTag, out AbstractGameplayState state)
    {
        state = null;
        if (PriorityStateMachines.TryGetValue(priorityTag, out GameplayStateMachine stateMachine))
        {
            state = stateMachine.CurrentState;
            return true;
        }

        return false;
    }

    public bool TryGetStoredState(StatePriorityTag priorityTag, AbstractGameplayStateScriptableObject sourceState, out AbstractGameplayState state)
    {
        state = null;
        foreach (AbstractGameplayState storedState in StoredStates[priorityTag].Where(storedState => storedState.GameplayState == sourceState))
        {
            state = storedState;
            return true;
        }

        return false;
    }

    public Dictionary<StatePriorityTag, AbstractGameplayState> GetActiveStates()
    {
        Dictionary<StatePriorityTag, AbstractGameplayState> activeStates = new Dictionary<StatePriorityTag, AbstractGameplayState>();
        foreach (StatePriorityTag priorityTag in PriorityStateMachines.Keys)
        {
            activeStates[priorityTag] = PriorityStateMachines[priorityTag].CurrentState;
        }

        return activeStates;
    }
    
    public void FillEmptyStates(StateModerator other)
    {
        foreach (StatePriorityTag priorityTag in other.PriorityStateMachines.Keys)
        {
            if (PriorityStateMachines.Keys.Contains(priorityTag)) continue;
            PriorityStateMachines[priorityTag] = new GameplayStateMachine();
            PriorityStateMachines[priorityTag].Initialize(other.PriorityStateMachines[priorityTag].CurrentState);
        }
    }

    public void RunStatesLogicUpdate()
    {
        foreach(GameplayStateMachine stateMachine in PriorityStateMachines.Values) stateMachine.CurrentState.LogicUpdate();
    }

    public void RunStatesPhysicsUpdate()
    {
        foreach(GameplayStateMachine stateMachine in PriorityStateMachines.Values) stateMachine.CurrentState.PhysicsUpdate();
    }

    public AbstractGameplayStateScriptableObject GetInitialState(StatePriorityTag priorityTag) =>
        BaseModerator.InitialStates.TryGetValue(priorityTag, out AbstractGameplayStateScriptableObject state) ? state : null;

    public bool BlockUndefinedTransitions => BaseModerator.BlockUndefinedTransitions;
    
    public List<AbstractGameplayStateScriptableObject> GetStatesByPriority(StatePriorityTag priorityTag) => BaseModerator.StatesByPriority.TryGetValue(priorityTag, out List<AbstractGameplayStateScriptableObject> states) ? states : null;
    
    public bool DefinesState(AbstractGameplayStateScriptableObject state) => BaseModerator.StatesByPriority.Keys.Any(priorityTag => BaseModerator.StatesByPriority[priorityTag].Contains(state));
    public bool DefinesState(StatePriorityTag priorityTag, AbstractGameplayStateScriptableObject state) => BaseModerator.StatesByPriority.TryGetValue(priorityTag, out List<AbstractGameplayStateScriptableObject> states) && states.Contains(state);
    
    public static bool operator >(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority > other.BaseModerator.ModeratorPriority;
    public static bool operator <(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority < other.BaseModerator.ModeratorPriority;
    public static bool operator >=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority >= other.BaseModerator.ModeratorPriority;
    public static bool operator <=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority <= other.BaseModerator.ModeratorPriority;

    public void ImplementMeta(MetaStateModerator metaModerator, bool reEnterSameStates)
    {
        Debug.Log($"Implementing {metaModerator.Moderator.name}");
        BaseModerator = metaModerator.Moderator;
        foreach (StatePriorityTag priorityTag in metaModerator.InitialStates.Keys)
        {
            if (!reEnterSameStates && PriorityStateMachines[priorityTag].CurrentState.GameplayState == metaModerator.InitialStates[priorityTag]) continue;
            InterruptChangeState(priorityTag, metaModerator.InitialStates[priorityTag]);
        }
    }
    
    public static MetaStateModerator GenerateMeta(StateModeratorScriptableObject moderator)
    {
        return new MetaStateModerator(moderator);
    }
    
    public class MetaStateModerator
    {
        public StateModeratorScriptableObject Moderator;
        public Dictionary<StatePriorityTag, AbstractGameplayStateScriptableObject> InitialStates;
        
        public MetaStateModerator(StateModeratorScriptableObject moderator)
        {
            Moderator = moderator;
            
            InitialStates = new Dictionary<StatePriorityTag, AbstractGameplayStateScriptableObject>();
            foreach (StatePriorityTag priorityTag in Moderator.InitialStates.Keys)
            {
                InitialStates[priorityTag] = Moderator.InitialStates[priorityTag];
            }
        }
    
        public bool DefinesState(StatePriorityTag priorityTag, AbstractGameplayStateScriptableObject state) => Moderator.StatesByPriority.TryGetValue(priorityTag, out List<AbstractGameplayStateScriptableObject> states) && states.Contains(state);
        public void ChangeState(StatePriorityTag priorityTag, AbstractGameplayStateScriptableObject newState)
        {
            InitialStates[priorityTag] = newState;
        }
        public void FillEmptyStates(StateModerator other)
        {
            foreach (StatePriorityTag priorityTag in other.PriorityStateMachines.Keys)
            {
                if (InitialStates.Keys.Contains(priorityTag)) continue;
                InitialStates[priorityTag] = other.PriorityStateMachines[priorityTag].CurrentState.GameplayState;
            }
        }
    }
}