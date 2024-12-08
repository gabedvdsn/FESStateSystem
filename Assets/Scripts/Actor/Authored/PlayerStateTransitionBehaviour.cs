using UnityEngine;

namespace FESStateSystem
{
    public class PlayerStateTransitionBehaviour : AbstractStateTransitionComponent<PlayerStateTransitionBehaviour>
    {

        public override void InitializeMatrix()
        {
            Matrix = TransitionMatrix.GenerateMatrix(this);
            Matrix.LogMatrix();
        }
    }
}
