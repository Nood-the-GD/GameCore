using System.Collections.Generic;
using UnityEngine;

namespace Core.Extension
{
    public static class ListExtension
    {
        public static T GetRandom<T>(this List<T> source)
        {
            var r = Random.Range(0, source.Count);
            return source[r];
        }
    }

    public static class ArrayExtension
    {
        public static T GetRadom<T>(this T[] source)
        {
            int r = Random.Range(0, source.Length);
            return source[r];
        }
    }
}