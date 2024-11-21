public abstract class AbstractPlayerGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    public abstract class AbstractPlayerGameplayState : AbstractGameplayState
    {
        public PlayerController Player;
        
        public override void Initialize(StateActor actor)
        {
            Player = actor.GetComponent<PlayerController>();
        }
        public AbstractPlayerGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
        {
            
        }
    }
}
