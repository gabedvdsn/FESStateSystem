using UnityEngine;

[CreateAssetMenu(menuName = "FESState/Priority Tag")]
public class StatePriorityTag : ScriptableObject
{
    public int Priority;
    public bool IsGreaterPriorityThan(StatePriorityTag other) => Priority > other.Priority;

    public static bool operator >(StatePriorityTag self, StatePriorityTag other) => self.Priority > other.Priority;
    public static bool operator <(StatePriorityTag self, StatePriorityTag other) => self.Priority < other.Priority;
    public static bool operator >=(StatePriorityTag self, StatePriorityTag other) => self.Priority >= other.Priority;
    public static bool operator <=(StatePriorityTag self, StatePriorityTag other) => self.Priority <= other.Priority;
}
