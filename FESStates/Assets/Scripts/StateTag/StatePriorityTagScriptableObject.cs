using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Priority Tag")]
public class StatePriorityTagScriptableObject : ScriptableObject
{
    public int Priority;
    public bool IsGreaterPriorityThan(StatePriorityTagScriptableObject other) => Priority < other.Priority;

    public static bool operator >(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority > other.Priority;
    public static bool operator <(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority < other.Priority;
    public static bool operator >=(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority >= other.Priority;
    public static bool operator <=(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority <= other.Priority;
}
