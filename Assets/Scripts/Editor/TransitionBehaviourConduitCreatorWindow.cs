using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FESStateSystem
{
    public class TransitionBehaviourConduitCreatorWindow : EditorWindow
    {
        private 
        private string savePath = "Assets/Scripts/AuthoredStateSystem/TransitionBehaviours";
        
        [MenuItem("StateSystem/Transition Behaviour Conduit Creator")]
        public static void ShowWindow()
        {
            GetWindow<TransitionBehaviourCreatorWindow>("Transition Behaviour Conduit Creator");
        }
    }
}
