using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Moderator")]
    public class StateModeratorScriptableObject : ScriptableObject
    {
        [FormerlySerializedAs("ModeratorPriority")] [Header("Priority")]
    
        public StateContextTagScriptableObject moderatorContext;

        [Header("States Manifest")] 
    
        public GameplayStateManifestScriptableObject Manifest;
    
        [Header("System Change Responders")]
    
        public List<AbstractSystemChangeResponseScriptableObject> Responders;

        public StateModerator GenerateModerator(StateActor actor) => new(this, actor);
    
    }

}