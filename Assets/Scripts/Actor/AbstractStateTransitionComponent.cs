using System;
using UnityEngine;

namespace FESStateSystem
{
    [RequireComponent(typeof(StateActor))]
    public abstract class AbstractStateTransitionComponent<S> : MonoBehaviour
    {
        public StateTransitionMatrixScriptableObject TransitionMatrix;
        
        protected StateActor State;
        protected StateTransitionMatrix<S> Matrix;

        private void Awake()
        {
            State = GetComponent<StateActor>();
            InitializeMatrix();
            
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        /// <summary>
        /// Initialize the Matrix field with the desired type parameter as S in the form:
        /// <example>Matrix = TransitionMatrix.GenerateMatrix(S);</example>
        /// </summary>
        public abstract void InitializeMatrix();
    }
}
