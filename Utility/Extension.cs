using System.Collections.Generic;
using System.Linq;
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
        public static T GetRandom<T>(this T[] source)
        {
            int r = Random.Range(0, source.Length);
            return source[r];
        }

        public static T GetRandom<T>(this T[] source, out int index)
        {
            index = Random.Range(0, source.Length);
            return source[index];
        }

        public static T GetRandom<T>(this T[] source, int[] exceptIndexes, out int index)
        {
            List<int> availableIndexes = new();

            for (int i = 0; i < source.Length; i++)
            {
                if (!exceptIndexes.Contains(i))
                {
                    availableIndexes.Add(i);
                }
            }

            index = Random.Range(0, availableIndexes.Count);
            return source[index];
        }

        public static T GetRandom<T>(this T[] source, T[] exceptions, out int index)
        {
            List<int> availableIndexes = new();

            for (int i = 0; i < source.Length; i++)
            {
                if (!exceptions.Contains(source[i]))
                {
                    availableIndexes.Add(i);
                }
            }

            index = Random.Range(0, availableIndexes.Count);
            return source[index];
        }
    }
}