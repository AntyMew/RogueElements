// <copyright file="LinkedDictionary.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RogueElements
{
    /// <summary>
    /// A collection of items indexable by keys and enumerable by insertion order.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class LinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<(TKey key, TValue value)>> dictionary;
        private readonly LinkedList<(TKey key, TValue value)> list;
        private KeyCollection keys;
        private ValueCollection values;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedDictionary{TKey, TValue}"/> class that is empty and has the specified
        /// initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements the new <see cref="LinkedDictionary{TKey, TValue}"/> can initially store.</param>
        public LinkedDictionary(int capacity = 0)
        {
            this.dictionary = new Dictionary<TKey, LinkedListNode<(TKey key, TValue value)>>(capacity);
            this.list = new LinkedList<(TKey key, TValue value)>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedDictionary{TKey, TValue}"/> class that contains the key-value pairs of the
        /// specified collection.
        /// </summary>
        /// <param name="pairs">The collection of key-value pairs to copy to the new <see cref="LinkedDictionary{TKey, TValue}"/>.</param>
        public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            : this(pairs.Count())
        {
            foreach (KeyValuePair<TKey, TValue> pair in pairs)
                this.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        public int Count => this.list.Count;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// An <see cref="ICollection{TKey}"/> of the keys in <see cref="LinkedDictionary{TKey, TValue}"/> ordered from oldest to newest
        /// added element.
        /// </value>
        public ICollection<TKey> Keys => this.keys ?? (this.keys = new KeyCollection(this));

        /// <summary>
        /// Gets a collection containing the values in the <see cref="LinkedDictionary{TKey, TValue}"/> ordered from oldest to newest
        /// added element.
        /// </summary>
        /// <value>An <see cref="ICollection{TValue}"/> of the values in <see cref="LinkedDictionary{TKey, TValue}"/>.</value>
        public ICollection<TValue> Values => this.values ?? (this.values = new ValueCollection(this));

        /// <summary>
        /// Gets a value indicating whether the <see cref="LinkedDictionary{TKey, TValue}"/> is read-only.
        /// </summary>
        /// <value>Always <see langword="true"/>.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;

        /// <summary>
        /// Gets or sets the element with the specified key in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="key"/> does not exist in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </exception>
        public TValue this[TKey key]
        {
            get => this.dictionary[key].Value.value;
            set
            {
                if (!this.ContainsKey(key))
                    throw new KeyNotFoundException($"Key does not exist in the {nameof(LinkedDictionary<TKey, TValue>)}.");
                else
                    this.dictionary[key].Value = (key, value);
            }
        }

        /// <summary>
        /// Adds a new element with the specified key to the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            if (this.dictionary.ContainsKey(key))
                throw new ArgumentException($"Key already exists in the {nameof(LinkedDictionary<TKey, TValue>)}.", nameof(key));
            var node = this.list.AddLast((key, value));
            this.dictionary.Add(key, node);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element was successfully removed; otherwise, <see langword="false"/>.</returns>
        /// /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool Remove(TKey key)
        {
            if (!this.dictionary.TryGetValue(key, out LinkedListNode<(TKey key, TValue value)> node))
            {
                return false;
            }
            else
            {
                this.dictionary.Remove(key);
                this.list.Remove(node);
                return true;
            }
        }

        /// <summary>
        /// Attempts to get the element with the specified key from the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to attempt to get.</param>
        /// <param name="value">
        /// If the method succeeded, contains the value of the element with the specified key; otherwise, contains the default value for
        /// <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="LinkedDictionary{TKey, TValue}"/> contains the <paramref name="key"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!this.dictionary.TryGetValue(key, out LinkedListNode<(TKey key, TValue value)> node))
            {
                value = default;
                return false;
            }
            else
            {
                value = node.Value.value;
                return true;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="LinkedDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="LinkedDictionary{TKey, TValue}"/> contains the <paramref name="key"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool ContainsKey(TKey key)
            => this.dictionary.ContainsKey(key);

        /// <summary>
        /// Removes all elements from the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            this.dictionary.Clear();
            this.list.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LinkedDictionary{TKey, TValue}"/> in the order of oldest to newest
        /// added element.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> object that can be used to iterate through the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => this.list.Select(p => new KeyValuePair<TKey, TValue>(p.key, p.value)).GetEnumerator();

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => this.Add(item.Key, item.Value);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if item was successfully removed from the <see cref="ICollection{T}"/>; otherwise,
        /// <see langword="false"/>. This method also returns false if item is not found in the original <see cref="ICollection{T}"/>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => this.Remove(item.Key);

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => this.TryGetValue(item.Key, out TValue value) && value.Equals(item.Value);

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{T}"/>.
        /// The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ICollection{T}"/> is greater than the available space from
        /// <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.AssertCanCopyTo(array, arrayIndex);

            foreach ((TKey key, TValue value) in this.list)
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

        private void AssertCanCopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < this.list.Count)
                throw new ArgumentException();
        }

        private class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;

            public KeyCollection(LinkedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int Count => this.dictionary.Count;

            public bool IsReadOnly => true;

            public bool Contains(TKey item)
                => this.dictionary.ContainsKey(item);

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                this.dictionary.AssertCanCopyTo(array, arrayIndex);

                foreach ((TKey key, TValue value) in this.dictionary.list)
                    array[arrayIndex++] = key;
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach ((TKey key, TValue value) in this.dictionary.list)
                    yield return key;
            }

            void ICollection<TKey>.Add(TKey item)
                => throw new NotSupportedException("Collection is readonly.");

            bool ICollection<TKey>.Remove(TKey item)
                => throw new NotSupportedException("Collection is readonly.");

            void ICollection<TKey>.Clear()
                => throw new NotSupportedException("Collection is readonly.");

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        private class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;

            public ValueCollection(LinkedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int Count => this.dictionary.Count;

            public bool IsReadOnly => true;

            public bool Contains(TValue value)
                => this.dictionary.Select(x => x.Value).Contains(value);

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                this.dictionary.AssertCanCopyTo(array, arrayIndex);

                foreach ((TKey key, TValue value) in this.dictionary.list)
                    array[arrayIndex++] = value;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach ((TKey key, TValue value) in this.dictionary.list)
                    yield return value;
            }

            void ICollection<TValue>.Add(TValue item)
                => throw new NotSupportedException("Collection is readonly.");

            bool ICollection<TValue>.Remove(TValue item)
                => throw new NotSupportedException("Collection is readonly.");

            void ICollection<TValue>.Clear()
                => throw new NotSupportedException("Collection is readonly.");

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}