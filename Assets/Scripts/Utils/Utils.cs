using System.Collections.Generic;
using UnityEngine;

namespace FESStateSystem
{
    public static class Utils
    {
        public static T RandomChoice<T>(this List<T> list) => list != null ? list[Mathf.FloorToInt(Random.value * list.Count)] : default;
        public static T Last<T>(this List<T> list) => list != null ? list[^1] : default;
    }
}
