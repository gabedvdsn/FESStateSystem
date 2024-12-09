using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        
            foreach (StateContextTagScriptableObject contextTag in BaseModerator.Manifest.Contexts)
            {
                ContextStateMachines[contextTag] = new GameplayStateMachine();
                AbstractGameplayState initialState = CreateStoreInitializeGameplayState(contextTag, BaseModerator.Manifest.InitialState(contextTag));
                ContextStateMachines[contextTag].Initialize(initialState);
            
                StateIsChanged(contextTag, null, initialState);
            
                foreach (AbstractGameplayStateScriptableObject state in BaseModerator.Manifest.Get(contextTag))
                {
                    if (state == initialState.StateData) continue;
                    CreateStoreInitializeGameplayState(contextTag, state);
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
        
        public void ReturnContextToInitial(AbstractGameplayState state)
        {
            if (!ContextStateMachines.ContainsKey(state.LiveContext))
            {
                // Cannot find context, return all priorities to initial states (hard reset)
                foreach (StateContextTagScriptableObject contextTag in ContextStateMachines.Keys)
                {
                    DefaultChangeState(contextTag, BaseModerator.Manifest.InitialState(contextTag));
                }

                return;
            }
        
            DefaultChangeState(state.LiveContext, BaseModerator.Manifest.InitialState(state.LiveContext));
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
            foreach (StateContextTagScriptableObject contextTag in ContextStateMachines.Keys)
            {
                activeStates[contextTag] = ContextStateMachines[contextTag].CurrentState;
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
            state.ProvideContext(contextTag);
            state.Initialize();

            // Never cache undefined states, in the case of forced state transitions
            if (!stateData.CacheState || !DefinesState(contextTag, stateData)) return state;
            
            if (!CachedStates.ContainsKey(contextTag)) CachedStates[contextTag] = new List<AbstractGameplayState>();
            CachedStates[contextTag].Add(state);

            return state;
        }
        
        public bool DefinesState(AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(state);

        public bool DefinesState(StateContextTagScriptableObject contextTag,
            AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(contextTag, state);

        public void ImplementModeratorMeta(StateModeratorMeta moderatorMeta, bool reEnterSameStates, bool interrupts)
        {
            ModeratorIsChanged(BaseModerator, moderatorMeta.BaseModerator);
            BaseModerator = moderatorMeta.BaseModerator;
        
            ApplyMetaImplementation(moderatorMeta, reEnterSameStates, interrupts);
        
            CleanupMetaImplementation();
        }

        private void ApplyMetaImplementation(StateModeratorMeta moderatorMeta, bool reEnterSameStates, bool interrupts)
        {
            foreach (StateContextTagScriptableObject contextTag in moderatorMeta.InitialStates.Keys)
            {
                // Add state machines for new priority levels
                if (!ContextStateMachines.ContainsKey(contextTag))
                {
                    ContextStateMachines[contextTag] = new GameplayStateMachine();
                    AbstractGameplayState initialState = CreateStoreInitializeGameplayState(contextTag, moderatorMeta.InitialStates[contextTag]);
                    ContextStateMachines[contextTag].Initialize(initialState);
                }
                // If the priority tag exists, handle changing state as necessary
                else
                {
                    if (!reEnterSameStates && ContextStateMachines[contextTag].CurrentState.StateData == moderatorMeta.InitialStates[contextTag]) continue;
                    
                    // Allow for undefined transitions; all validation logic has been done before this step
                    if (interrupts) InterruptChangeState(contextTag, moderatorMeta.InitialStates[contextTag], false);
                    else DefaultChangeState(contextTag, moderatorMeta.InitialStates[contextTag], false);
                }

                // Store new states 
                foreach (AbstractGameplayStateScriptableObject sourceState in moderatorMeta.BaseModerator.Manifest.Get(contextTag))
                {
                    if (!TryGetCachedState(contextTag, sourceState, out AbstractGameplayState _)) CreateStoreInitializeGameplayState(contextTag, sourceState);
                }
            }
        }

        private void CleanupMetaImplementation()
        {
        
            // Dump state machines for unused priority levels
            List<StateContextTagScriptableObject> machinesToRemove = ContextStateMachines.Keys.Where(contextTag => !BaseModerator.Manifest.Contexts.Contains(contextTag)).ToList();
            foreach (StateContextTagScriptableObject contextTag in machinesToRemove)
            {
                ContextStateMachines.Remove(contextTag);
                CachedStates.Remove(contextTag);
            }

            // Dump unusable states
            foreach (StateContextTagScriptableObject contextTag in CachedStates.Keys)
            {
                List<AbstractGameplayState> statesToRemove = CachedStates[contextTag].Where(storedState => !DefinesState(contextTag, storedState.StateData)).ToList();
                foreach (AbstractGameplayState state in statesToRemove) CachedStates[contextTag].Remove(state);
            }
        }
    
        public static StateModeratorMeta GenerateMeta(StateModeratorScriptableObject moderator)
        {
            return new StateModeratorMeta(moderator);
        }
    
        public class StateModeratorMeta
        {
            public StateModeratorScriptableObject BaseModerator;
            public Dictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject> InitialStates;
        
            public StateModeratorMeta(StateModeratorScriptableObject baseModerator)
            {
                BaseModerator = baseModerator;
            
                InitialStates = new Dictionary<StateContextTagScriptableObject, AbstractGameplayStateScriptableObject>();
                foreach (StateContextTagScriptableObject contextTag in BaseModerator.Manifest.Contexts)
                {
                    InitialStates[contextTag] = BaseModerator.Manifest.InitialState(contextTag);
                }
            }

            public bool DefinesState(StateContextTagScriptableObject contextTag,
                AbstractGameplayStateScriptableObject state) => BaseModerator.Manifest.DefinesState(contextTag, state);
            public void ChangeInitialState(StateContextTagScriptableObject contextTag, AbstractGameplayStateScriptableObject newState)
            {
                InitialStates[contextTag] = newState;
            }
            public void FillEmptyStates(StateModerator other)
            {
                foreach (StateContextTagScriptableObject contextTag in other.ContextStateMachines.Keys)
                {
                    if (InitialStates.Keys.Contains(contextTag)) continue;
                    InitialStates[contextTag] = other.ContextStateMachines[contextTag].CurrentState.StateData;
                }
            }
        }
    }
}
