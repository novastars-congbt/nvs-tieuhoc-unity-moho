using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.Anagram
{
    /// <summary>
    /// Holds a collection of words that are anagrams of each other.
    /// </summary>
    [Serializable]
    public class AnagramCollection : IList<string>
    {
        /// <summary>
        /// List of words.
        /// </summary>
        [SerializeField]
        private List<string> _list = new List<string>();

        /// <summary>
        /// Exposes the indexer of the list.
        /// </summary>
        /// <param name="index">Index at which you want to get the object.</param>
        /// <returns>Object at the give index.</returns>
        public string this[int index] { get => _list[index]; set => _list[index] = value; }

        /// <summary>
        /// Gets the amount of anagrams in the collection.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Whether the collection is read only or not.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Adds a word to the collection.
        /// </summary>
        /// <param name="item">Word to be added.</param>
        public void Add(string item) => _list.Add(item);

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear() => _list.Clear();

        /// <summary>
        /// Checks if the word is in this collection.
        /// </summary>
        /// <param name="item">Word.</param>
        /// <returns>Whether it is in the collection.</returns>
        public bool Contains(string item) => _list.Contains(item);

        /// <summary>
        /// Copies elements to an array.
        /// </summary>
        /// <param name="array">Goal array.</param>
        /// <param name="arrayIndex">Index where copying begins.</param>
        public void CopyTo(string[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets the IEnumerator of the list.
        /// </summary>
        public IEnumerator<string> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// Gets the index of a given word in the collection.
        /// </summary>
        /// <param name="item">Word in the collection.</param>
        /// <returns>Index of the word.</returns>
        public int IndexOf(string item) => _list.IndexOf(item);

        /// <summary>
        /// Inserts a word into the given index.
        /// </summary>
        /// <param name="index">Where the word will be added.</param>
        /// <param name="item">Word to be added.</param>
        public void Insert(int index, string item) => _list.Insert(index, item);

        /// <summary>
        /// Removes a word from the collection.
        /// </summary>
        /// <param name="item">Word to be removed.</param>
        /// <returns>Whether it was removed.</returns>
        public bool Remove(string item) => _list.Remove(item);

        /// <summary>
        /// Removes the word at the given index from the collection.
        /// </summary>
        /// <param name="index">Index of the word.</param>
        public void RemoveAt(int index) => _list.RemoveAt(index);

        /// <summary>
        /// Gets the IEnumerator of the list.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}