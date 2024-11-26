using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESStates/State/Manifest")]
public class GameplayStateManifestScriptableObject : ScriptableObject
{
    public SerializedDictionary<StatePriorityTagScriptableObject, GameplayStateGroupScriptableObject> PriorityManifest;
}
