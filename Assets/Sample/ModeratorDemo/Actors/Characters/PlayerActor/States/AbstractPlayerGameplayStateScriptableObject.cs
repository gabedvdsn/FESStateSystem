namespace FESStateSystem.ModeratorDemo
{
    public abstract class AbstractPlayerGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
    {
        public abstract class AbstractPlayerGameplayState : AbstractGameplayState
        {
            public DemoPlayerController Player;
        
            public AbstractPlayerGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
            {
            
            }
        
            public override void Initialize()
            {
                Player = State.GetComponent<DemoPlayerController>();
            }
        }
    }
}