using System.Collections.Generic;
using System.Linq;

namespace FESStateSystem
{
    public class StateModerator
    {
        public StateModeratorScriptableObject BaseModerator;
        public StateActor StateComponent;

        private Dictionary<StateContextTagScriptableObject, GameplayStateMachine> ContextStateMachines;
        private Dictionary<StateContextTagScriptableObject, List<AbstractGameplayState>> CachedStates;
    
        public StateModerator(StateModeratorScriptableObject moderator, StateActor stateComponent)
        {
            BaseModerator = moderator;
            StateComponent = stateComponent;
        
            InitializeModeratorStates();
        }

        private void InitializeModeratorStates()
        {
            CachedStates = new Dictionary<StateContextTagScriptableObject, List<AbstractGameplayState>>();
            ContextStateMachines = new Dictionary<StateContextTagScriptableObject, GameplayStateMachine>();
        
            foreach (StateContextTagScriptableObject priorityTag in BaseModerator.Manifest.Contexts)
            {
                ContextStateMachines[priorityTag] = new GameplayStateMachine();
                AbstractGameplayState initialState = CreateStoreInitializeGameplayState(priorityTag, BaseModerator.Manifest.InitialState(priorityTag));
                ContextStateMachines[priorityTag].Initialize(initialState);
            
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
        /// <param name="contextTag">The priority tag of the state to change.</param>
        /// <param name="newState">The state to change to.</param>
        /// <param name="onlyDefined">Disallow non-defined states.</param>
        public void DefaultChangeState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState, bool onlyDefined = true)
        {
            if (!DefinesState(contextTag, newState) && onlyDefined) return;
            if (!TryGetCachedState(contextTag, newState, out AbstractGameplayState state)) state = CreateStoreInitializeGameplayState(contextTag, newState);
        
            StateIsChanged(contextTag, ContextStateMachines[contextTag].CurrentState, state);
            ContextStateMachines[contextTag].ChangeState(state);
        }

        /// <summary>
        /// Interrupts and changes the active state under some priority tag. Mutually exclusive from DefaultChangeState()
        /// </summary>
        /// <param name="contextTag">The priority tag of the state to change.</param>
        /// <param name="newState">The state to change to.</param>
        /// <param name="onlyDefined">Disallow non-defined states.</param>
        public void InterruptChangeState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState, bool onlyDefined = true)
        {
            if (!DefinesState(contextTag, newState) && onlyDefined) return;
            if (!TryGetCachedState(contextTag, newState, out AbstractGameplayState state)) state = CreateStoreInitializeGameplayState(contextTag, newState);
        
            StateIsChanged(contextTag, ContextStateMachines[contextTag].CurrentState, state);
            ContextStateMachines[contextTag].InterruptChangeState(state);
        }

        private void StateIsChanged(StateContextTagScriptableObject contextTag, AbstractGameplayState oldState, AbstractGameplayState newState)
        {
            foreach (AbstractSystemChangeResponseScriptableObject systemChangeResponse in BaseModerator.Responders)
                systemChangeResponse.OnStateChanged(StateComponent, contextTag, oldState, newState);
        }

        private void ModeratorIsChanged(StateModeratorScriptableObject oldModerator, StateModeratorScriptableObject newModerator)
        {
            foreach (AbstractSystemChangeResponseScriptableObject moderatorChangeBehaviour in BaseModerator.Responders) moderatorChangeBehaviour.OnModeratorChanged(StateComponent, oldModerator, newModerator);
        }

        public void ReturnToInitial(AbstractGameplayStateScriptableObject sourceState)
        {
            foreach (StateContextTagScriptableObject priorityTag in CachedStates.Keys)
            {
                if (CachedStates[priorityTag].Any(state => state.StateData == sourceState))
                {
                    DefaultChangeState(priorityTag, BaseModerator.Manifest.InitialState(priorityTag));
                    return;
                }
            }
        
            // Cannot find the sourceState, return all priorities to initial states (hard reset)
            foreach (StateContextTagScriptableObject priorityTag in ContextStateMachines.Keys)
            {
                DefaultChangeState(priorityTag, BaseModerator.Manifest.InitialState(priorityTag));
            }
        }

        public bool TryGetActiveStatePriority(AbstractGameplayStateScriptableObject state, out StateContextTagScriptableObject stateContextTag)
        {
            stateContextTag = null;
            foreach (StateContextTagScriptableObject priorityTag in CachedStates.Keys.Where(priorityTag => CachedStates[priorityTag].Any(storedState => storedState.StateData == state)))
            {
                stateContextTag = priorityTag;
                return true;
            }

            return false;
        }

        public bool TryGetActiveState(StateContextTagScriptableObject contextTag, out AbstractGameplayState state)
        {
            state = null;
            if (!ContextStateMachines.ContainsKey(contextTag)) return false;
            if (ContextStateMachines.TryGetValue(contextTag, out GameplayStateMachine stateMachine))
            {
                state = stateMachine.CurrentState;
                return true;
            }

            return false;
        }

        public bool TryGetCachedState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject sourceState, out AbstractGameplayState state)
        {
            state = null;
            if (!CachedStates.ContainsKey(contextTag)) return false;
            foreach (AbstractGameplayState storedState in CachedStates[contextTag].Where(storedState => storedState.StateData == sourceState))
            {
                state = storedState;
                return true;
            }

            return false;
        }

        public List<AbstractGameplayState> GetActiveStates()
        {
            return ContextStateMachines.Values.Select(machine => machine.CurrentState).ToList();
        }

        public Dictionary<StateContextTagScriptableObject, AbstractGameplayState> GetActiveStatesWithPriority()
        {
            Dictionary<StateContextTagScriptableObject, AbstractGameplayState> activeStates = new Dictionary<StateContextTagScriptableObject, AbstractGameplayState>();
            foreach (StateContextTagScriptableObject priorityTag in ContextStateMachines.Keys)
            {
                activeStates[priorityTag] = ContextStateMachines[priorityTag].CurrentState;
            }

            return activeStates;
        }

        public void RunStatesLogicUpdate()
        {
            foreach(GameplayStateMachine stateMachine in ContextStateMachines.Values) stateMachine.CurrentState.LogicUpdate();
        }

        public void RunStatesPhysicsUpdate()
        {
            foreach(GameplayStateMachine stateMachine in ContextStateMachines.Values) stateMachine.CurrentState.PhysicsUpdate();
        }
        
        private AbstractGameplayState CreateStoreInitializeGameplayState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject stateData)
        {
            if (TryGetCachedState(contextTag, stateData, out AbstractGameplayState state)) return state;
            
            state = stateData.GenerateStates(StateComponent)[0];
            state.Initialize();

            if (!stateData.CacheState) return state;
            
            if (!CachedStates.ContainsKey(contextTag)) CachedStates[contextTag] = new List<AbstractGameplayState>();
            CachedStates[contextTag].Add(state);

            return state;
        }

        public bool DefinesState(AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(state);

        public bool DefinesState(StateContextTagScriptableObject contextTag,
            AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(contextTag, state);
    
        public static bool operator >(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorContext > other.BaseModerator.ModeratorContext;
        public static bool operator <(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorContext < other.BaseModerator.ModeratorContext;
        public static bool operator >=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorContext >= other.BaseModerator.ModeratorContext;
        public static bool operator <=(StateModerator self, StateModerator other) => self.BaseModerator.ModeratorContext <= other.BaseModerator.ModeratorContext;

        public void ImplementModeratorMeta(MetaStateModerator metaModerator, bool reEnterSameStates, bool interrupts)
        {
            ModeratorIsChanged(BaseModerator, metaModerator.BaseModerator);
            BaseModerator = metaModerator.BaseModerator;
        
            ApplyMetaImplementation(metaModerator, reEnterSameStates, interrupts);
        
            CleanupMetaImplementation();
        }

        private void ApplyMetaImplementation(MetaStateModerator metaModerator, bool reEnterSameStates, bool interrupts)
        {
            foreach (StateContextTagScriptableObject priorityTag in metaModerator.InitialStates.Keys)
            {
                // Add state machines for new priority levels
                if (!ContextStateMachines.ContainsKey(priorityTag))
                {
                    ContextStateMachines[priorityTag] = new GameplayStateMachine();
                    AbstractGameplayState initialState = CreateStoreInitializeGameplayState(priorityTag, metaModerator.InitialStates[priorityTag]);
                    ContextStateMachines[priorityTag].Initialize(initialState);
                }
                // If the priority tag exists, handle changing state as necessary
                else
                {
                    if (!reEnterSameStates && ContextStateMachines[priorityTag].CurrentState.StateData == metaModerator.InitialStates[priorityTag]) continue;
                    if (interrupts) InterruptChangeState(priorityTag, metaModerator.InitialStates[priorityTag]);
                    else DefaultChangeState(priorityTag, metaModerator.InitialStates[priorityTag]);
                }

                // Store new states 
                foreach (AbstractGameplayStateScriptableObject sourceState in metaModerator.BaseModerator.Manifest.Get(priorityTag))
                {
                    if (!TryGetCachedState(priorityTag, sourceState, out AbstractGameplayState _)) CreateStoreInitializeGameplayState(priorityTag, sourceState);
                }
            }
        }

        private void CleanupMetaImplementation()
        {
        
            // Dump state machines for unused priority levels
            List<StateContextTagScriptableObject> machinesToRemove = ContextStateMachines.Keys.Where(priorityTag => !BaseModerator.Manifest.Contexts.Contains(priorityTag)).ToList();
            foreach (StateContextTagScriptableObject priorityTag in machinesToRemove)
            {
                ContextStateMachines.Remove(priorityTag);
                CachedStates.Remove(priorityTag);
            }

            // Dump unusable states
            foreach (StateContextTagScriptableObject priorityTag in CachedStates.Keys)
            {
                List<AbstractGameplayState> statesToRemove = CachedStates[priorityTag].Where(storedState => !DefinesState(priorityTag, storedState.StateData)).ToList();
                foreach (AbstractGameplayState state in statesToRemove) CachedStates[priorityTag].Remove(state);
            }
        }
    
        public static MetaStateModerator GenerateMeta(StateModeratorScriptableObject moderator)
        {
            return new MetaStateModerator(moderator);
        }
    
        public class MetaStateModerator
        {
            public StateModeratorScriptableObject BaseModerator;
            public Dictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject> InitialStates;
        
            public MetaStateModerator(StateModeratorScriptableObject baseModerator)
            {
                BaseModerator = baseModerator;
            
                InitialStates = new Dictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject>();
                foreach (StateContextTagScriptableObject priorityTag in BaseModerator.Manifest.Contexts)
                {
                    InitialStates[priorityTag] = BaseModerator.Manifest.InitialState(priorityTag);
                }
            }

            public bool DefinesState(StateContextTagScriptableObject contextTag,
                AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(contextTag, state);
            public void ChangeState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState)
            {
                InitialStates[contextTag] = newState;
            }
            public void FillEmptyStates(StateModerator other)
            {
                foreach (StateContextTagScriptableObject priorityTag in other.ContextStateMachines.Keys)
                {
                    if (InitialStates.Keys.Contains(priorityTag)) continue;
                    InitialStates[priorityTag] = other.ContextStateMachines[priorityTag].CurrentState.StateData;
                }
            }
        }
    }
}
