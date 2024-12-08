using UnityEngine;
using FESStateSystem;

[RequireComponent(typeof(DemoPlayerController))]
public class DemoPlayerStateTransitionBehaviour : AbstractStateTransitionComponent<DemoPlayerController>
{
    public override void InitializeMatrix()
    {
        Matrix = TransitionMatrix.GenerateMatrix(GetComponent<DemoPlayerController>());
        if (Log) Matrix.LogMatrix();
    }

    protected override AbstractGameplayStateScriptableObject GetSelectTransition(StateContextTagScriptableObject contextTag, TransitionEvaluationResult result)
    {
        return base.GetSelectTransition(contextTag, result);
    }
}