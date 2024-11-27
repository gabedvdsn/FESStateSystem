using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "FESState/State/Manifest")]
public class GameplayStateManifestScriptableObject : ScriptableObject
{
    [Tooltip("The first entry for each priority tag is each priority's initial state.")]
    [SerializedDictionary("Priority Tag", "Permitted States")]
    public SerializedDictionary<StatePriorityTagScriptableObject, List<AbstractGameplayStateScriptableObject>>
        PriorityManifest;

    public StatePriorityTagScriptableObject[] Priorities => PriorityManifest.Keys.ToArray();

    public AbstractGameplayStateScriptableObject InitialState(StatePriorityTagScriptableObject priorityTag) =>
        PriorityManifest[priorityTag][0];

    public List<AbstractGameplayStateScriptableObject> Get(StatePriorityTagScriptableObject priorityTag) =>
        PriorityManifest[priorityTag];

    public bool DefinesState(AbstractGameplayStateScriptableObject state)
    {
        return Priorities.Any(p => PriorityManifest[p].Contains(state));
    }

    public bool DefinesState(StatePriorityTagScriptableObject priorityTag, AbstractGameplayStateScriptableObject state)
    {
        return PriorityManifest[priorityTag].Contains(state);
    }

}