using UnityEngine;

namespace FESStateSystem
{
    [CreateAssetMenu(menuName = "FESState/State/Tag/Context")]
    public class StateContextTagScriptableObject : ScriptableObject
    {
        public int Priority;
        public bool IsGreaterPriorityThan(StateContextTagScriptableObject other) => Priority < other.Priority;

        public static bool operator >(StateContextTagScriptableObject self, StateContextTagScriptableObject other) => self.Priority > other.Priority;
        public static bool operator <(StateContextTagScriptableObject self, StateContextTagScriptableObject other) => self.Priority < other.Priority;
        public static bool operator >=(StateContextTagScriptableObject self, StateContextTagScriptableObject other) => self.Priority >= other.Priority;
        public static bool operator <=(StateContextTagScriptableObject self, StateContextTagScriptableObject other) => self.Priority <= other.Priority;
    }
}