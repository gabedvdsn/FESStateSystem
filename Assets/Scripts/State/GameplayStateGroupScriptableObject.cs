using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/State/States Group")]
public class GameplayStateGroupScriptableObject : ScriptableObject
{
    public List<AbstractGameplayStateScriptableObject> States;
    
    
}
