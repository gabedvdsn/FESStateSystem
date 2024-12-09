using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Moderator")]
    public class StateModeratorScriptableObject : ScriptableObject
    {
        [Header("States Manifest")] 
    
        public GameplayStateManifestScriptableObject Manifest;
    
        [Header("System Change Responders")]
    
        public List<AbstractSystemChangeResponseScriptableObject> Responders;

        public StateModerator GenerateModerator(StateActor actor) => new(this, actor);
    
    }

}