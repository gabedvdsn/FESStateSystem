using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/State/Manifest")]
public class GameplayStateManifestScriptableObject : ScriptableObject
{
    public SerializedDictionary<StatePriorityTagScriptableObject, GameplayStateGroupScriptableObject> PriorityManifest;
}
