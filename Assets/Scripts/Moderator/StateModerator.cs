using System.Collections.Generic;
using System.Linq;

namespace FESStateSystem
{
    public class StateModerator
    {
        public StateModeratorScriptableObject BaseModerator;
        public StateActor StateComponent;

        private Dictionary<StatePriorityTagScriptableObject, GameplayStateMachine> PriorityStateMachines;
        private Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayState>> StoredStates;
    
        public StateModerator(StateModeratorScriptableObject moderator, StateActor stateComponent)
        {
            BaseModerator = moderator;
            StateComponent = stateComponent;
        
            InitializeModeratorStates();
        }

        private void InitializeModeratorStates()
        {
            StoredStates = new Dictionary<StatePriorityTagScriptableObject, List<AbstractGameplayState>>();
            PriorityStateMachines = new Dictionary<StatePriorityTagScriptableObject, GameplayStateMachine>();
        
            foreach (StatePriorityTagScriptableObject priorityTag in BaseModerator.Manifest.Priorities)
            {
                PriorityStateMachines[priorityTag] = new GameplayStateMachine();
                AbstractGameplayState initialState = CreateStoreInitializeGameplayState(priorityTag, BaseModerator.Manifest.InitialState(priorityTag));
                PriorityStateMachines[priorityTag].Initialize(initialState);
            
                StateIsChanged(priorityTag, null, initialState);
            
                foreach (AbstractGameplayStateScriptableObject state in BaseModerator.Manifest.Get(priorityTag))
                {
                    if (state == initialState.StateData) continue;
                    CreateStoreInitializeGameplayState(priorityTag, state);
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
            if (!TryGetStoredState(priorityTag, newState, out AbstractGameplayState state)) state = CreateStoreInitializeGameplayState(priorityTag, newState);
        
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
            if (!TryGetStoredState(priorityTag, newState, out AbstractGameplayState state)) state = CreateStoreInitializeGameplayState(priorityTag, newState);
        
            StateIsChanged(priorityTag, PriorityStateMachines[priorityTag].CurrentState, state);
            PriorityStateMachines[priorityTag].InterruptChangeState(state);
        }

        private void StateIsChanged(StatePriorityTagScriptableObject priorityTag, AbstractGameplayState oldState, AbstractGameplayState newState)
        {
            foreach (AbstractSystemChangeResponseScriptableObject systemChangeResponse in BaseModerator.Responders)
                systemChangeResponse.OnStateChanged(StateComponent, priorityTag, oldState, newState);
        }

        private void ModeratorIsChanged(StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
        {
            foreach (AbstractSystemChangeResponseScriptableObject moderatorChangeBehaviour in BaseModerator.Responders) moderatorChangeBehaviour.OnModeratorChanged(StateComponent, oldModerator, newModerator);
        }

        public void ReturnToInitial(AbstractGameplayStateScriptableObject sourceState)
        {
            foreach (StatePriorityTagScriptableObject priorityTag in StoredStates.Keys)
            {
                if (StoredStates[priorityTag].Any(state => state.StateData == sourceState))
                {
                    DefaultChangeState(priorityTag, BaseModerator.Manifest.InitialState(priorityTag));
                    return;
                }
            }
        
            // Cannot find the sourceState, return all priorities to initial states (hard reset)
            foreach (StatePriorityTagScriptableObject priorityTag in PriorityStateMachines.Keys)
            {
                DefaultChangeState(priorityTag, BaseModerator.Manifest.InitialState(priorityTag));
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
            if (!PriorityStateMachines.ContainsKey(priorityTag)) return false;
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
            if (!StoredStates.ContainsKey(priorityTag)) return false;
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
    
        public List<AbstractGameplayStateScriptableObject> GetStatesByPriority(StatePriorityTagScriptableObject priorityTag) => BaseModerator.Manifest.PriorityManifest.GetValueOrDefault(priorityTag);

        public AbstractGameplayState CreateStoreInitializeGameplayState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject stateData)
        {
            if (TryGetStoredState(priorityTag, stateData, out AbstractGameplayState state)) return state;
        
            state = stateData.GenerateState(StateComponent);
            state.Initialize();
        
            if (!StoredStates.ContainsKey(priorityTag)) StoredStates[priorityTag] = new List<AbstractGameplayState>();
            StoredStates[priorityTag].Add(state);
        
            return state;
        }

        public bool DefinesState(AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(state);

        public bool DefinesState(StatePriorityTagScriptableObject priorityTag,
            AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(priorityTag, state);
    
        public static bool operator >(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority > other.BaseModerator.ModeratorPriority;
        public static bool operator <(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority < other.BaseModerator.ModeratorPriority;
        public static bool operator >=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority >= other.BaseModerator.ModeratorPriority;
        public static bool operator <=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorPriority <= other.BaseModerator.ModeratorPriority;

        public void ImplementModeratorMeta(MetaStateModerator metaModerator, bool reEnterSameStates, bool interrupts)
        {
            ModeratorIsChanged(BaseModerator, metaModerator.BaseModerator);
            BaseModerator = metaModerator.BaseModerator;
        
            ApplyMetaImplementation(metaModerator, reEnterSameStates, interrupts);
        
            CleanupMetaImplementation();
        }

        private void ApplyMetaImplementation(MetaStateModerator metaModerator, bool reEnterSameStates, bool interrupts)
        {
            foreach (StatePriorityTagScriptableObject priorityTag in metaModerator.InitialStates.Keys)
            {
                // Add state machines for new priority levels
                if (!PriorityStateMachines.ContainsKey(priorityTag))
                {
                    PriorityStateMachines[priorityTag] = new GameplayStateMachine();
                    AbstractGameplayState initialState = CreateStoreInitializeGameplayState(priorityTag, metaModerator.InitialStates[priorityTag]);
                    PriorityStateMachines[priorityTag].Initialize(initialState);
                }
                // If the priority tag exists, handle changing state as necessary
                else
                {
                    if (!reEnterSameStates && PriorityStateMachines[priorityTag].CurrentState.StateData == metaModerator.InitialStates[priorityTag]) continue;
                    if (interrupts) InterruptChangeState(priorityTag, metaModerator.InitialStates[priorityTag]);
                    else DefaultChangeState(priorityTag, metaModerator.InitialStates[priorityTag]);
                }

                // Store new states 
                foreach (AbstractGameplayStateScriptableObject sourceState in metaModerator.BaseModerator.Manifest.Get(priorityTag))
                {
                    if (!TryGetStoredState(priorityTag, sourceState, out AbstractGameplayState _)) CreateStoreInitializeGameplayState(priorityTag, sourceState);
                }
            }
        }

        private void CleanupMetaImplementation()
        {
        
            // Dump state machines for unused priority levels
            List<StatePriorityTagScriptableObject> machinesToRemove = PriorityStateMachines.Keys.Where(priorityTag => !BaseModerator.Manifest.Priorities.Contains(priorityTag)).ToList();
            foreach (StatePriorityTagScriptableObject priorityTag in machinesToRemove)
            {
                PriorityStateMachines.Remove(priorityTag);
                StoredStates.Remove(priorityTag);
            }

            // Dump unusable states
            foreach (StatePriorityTagScriptableObject priorityTag in StoredStates.Keys)
            {
                List<AbstractGameplayState> statesToRemove = StoredStates[priorityTag].Where(storedState => !DefinesState(priorityTag, storedState.StateData)).ToList();
                foreach (AbstractGameplayState state in statesToRemove) StoredStates[priorityTag].Remove(state);
            }
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
                foreach (StatePriorityTagScriptableObject priorityTag in BaseModerator.Manifest.Priorities)
                {
                    InitialStates[priorityTag] = BaseModerator.Manifest.InitialState(priorityTag);
                }
            }

            public bool DefinesState(StatePriorityTagScriptableObject priorityTag,
                AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(priorityTag, state);
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
}
