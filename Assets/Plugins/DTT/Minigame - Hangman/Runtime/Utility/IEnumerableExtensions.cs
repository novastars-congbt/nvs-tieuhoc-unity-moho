using System.Collections.Generic;
using UnityEngine;

namespace DTT.Hangman
{
    /// <summary>
    /// Provides extension methods for IEnumerable{T} implementations.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Takes a given amount of random entries.
        /// </summary>
        /// <param name="enumerable">The enumerable to take the random entries from.</param>
        /// <param name="count">The number of entries to take.</param>
        /// <typeparam name="T">The type of enumerable.</typeparam>
        /// <returns>The random entries.</returns>
        public static T[] TakeRandom<T>(this IEnumerable<T> enumerable, int count)
        {
            List<T> all = new List<T>(enumerable);
            T[] random = new T[count];
            
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, all.Count);
                random[i] = all[randomIndex];
                
                all.RemoveAt(randomIndex);
            }
            
            return random;
        }
    }
}