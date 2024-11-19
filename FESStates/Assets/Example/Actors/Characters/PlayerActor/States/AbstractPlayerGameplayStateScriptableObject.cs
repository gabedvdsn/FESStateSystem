public abstract class AbstractPlayerGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
{
    public abstract class AbstractPlayerGameplayState : AbstractGameplayState
    {
        public PlayerStateActor Player;
        
        public AbstractPlayerGameplayState(AbstractGameplayStateScriptableObject stateData, PlayerStateActor actor) : base(stateData)
        {
            Player = actor;
        }
    }
}
