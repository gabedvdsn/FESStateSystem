public abstract class AbstractPlayerPassiveBehaviourScriptableObject : AbstractPassiveStateBehaviourScriptableObject
{
    public abstract class AbstractPlayerPassiveStateBehaviour : AbstractPassiveStateBehaviour
    {
        public PlayerStateActor Player;
        protected AbstractPlayerPassiveStateBehaviour(PlayerStateActor player)
        {
            Player = player;
        }
    }
}
