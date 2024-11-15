public abstract class AbstractPlayerGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    public abstract class AbstractPlayerGameplayState : AbstractGameplayState
    {
        public PlayerStateActor Player;
        
        public AbstractPlayerGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState)
        {
            Player = actor;
        }
    }
}
