using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Actor/Player/Passive/Rotate")]
public class PlayerRotatePassiveBehaviourScriptableObject : AbstractPlayerPassiveBehaviourScriptableObject
{
    public Vector3 RotateAmount;
    
    public override AbstractPassiveStateBehaviour Generate(StateActor actor)
    {
        return new PlayerRotatePassiveBehaviour(this, actor as PlayerStateActor);
    }

    public class PlayerRotatePassiveBehaviour : AbstractPlayerPassiveStateBehaviour
    {
        private PlayerRotatePassiveBehaviourScriptableObject RotateBehaviour;
        
        public PlayerRotatePassiveBehaviour(AbstractPassiveStateBehaviourScriptableObject behaviour, PlayerStateActor actor) : base(actor)
        {
            RotateBehaviour = behaviour as PlayerRotatePassiveBehaviourScriptableObject;
        }
        
        public override void OnEnter()
        {
        }
        
        public override void OnUpdate()
        {
            Player.transform.Rotate(RotateBehaviour.RotateAmount * Time.deltaTime);
        }
        
        public override void OnPhysicsUpdate()
        {
        }
        
        public override void OnLeave()
        {
        }
    } 
}
