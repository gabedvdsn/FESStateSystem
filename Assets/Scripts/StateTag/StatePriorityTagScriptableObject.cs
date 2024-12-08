using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Tag/Priority")]
    public class StatePriorityTagScriptableObject : ScriptableObject
    {
        public int Priority;
        public bool IsGreaterPriorityThan(StatePriorityTagScriptableObject other) => Priority < other.Priority;

        public static bool operator >(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority > other.Priority;
        public static bool operator <(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority < other.Priority;
        public static bool operator >=(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority >= other.Priority;
        public static bool operator <=(StatePriorityTagScriptableObject self, StatePriorityTagScriptableObject other) => self.Priority <= other.Priority;
    }
}