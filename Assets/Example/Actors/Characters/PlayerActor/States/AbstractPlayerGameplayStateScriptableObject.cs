public abstract class AbstractPlayerGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    public abstract class AbstractPlayerGameplayState : AbstractGameplayState
    {
        public PlayerController Player;
        
        public override void Initialize()
        {
            Player = State.GetComponent<PlayerController>();
        }
        public AbstractPlayerGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {
            
        }
    }
}
