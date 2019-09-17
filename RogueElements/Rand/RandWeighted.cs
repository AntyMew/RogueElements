// <copyright file="RandWeighted.cs" company="Audino">
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
    /// A collection of items which can randomly pick any item with a probability proportional to its weight.
    ///
    /// <seealso cref="RandBag{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the item to pick.</typeparam>
    public class RandWeighted<T> : IDictionary<T, int>, IRandPicker<T>
    {
        private readonly LinkedDictionary<T, int> items;
        private readonly List<IRandPicker<T>> groups;
        private int sum;
        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandWeighted{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements the new <see cref="RandWeighted{T}"/> can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public RandWeighted(int capacity = 0)
        {
            this.items = new LinkedDictionary<T, int>(capacity);
            this.groups = new List<IRandPicker<T>>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandWeighted{T}"/> class that contains the key-value pairs of the specified
        /// collection.
        /// </summary>
        /// <param name="pairs">The collection of key-value pairs to copy to the new <see cref="RandWeighted{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">One or more of the weights is less than 1.</exception>
        public RandWeighted(IEnumerable<KeyValuePair<T, int>> pairs)
            : this(pairs.Count())
        {
            foreach (KeyValuePair<T, int> pair in pairs)
                this.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandWeighted{T}"/> class that contains a copy of the state of the other instance.
        /// </summary>
        /// <param name="other">The <see cref="RandWeighted{T}"/> instance to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        protected RandWeighted(RandWeighted<T> other)
            : this(other.items)
        {
            this.groups = other.groups.ToList();
            this.initialized = other.initialized;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="RandWeighted{T}"/>.
        /// </summary>
        public int Count => this.items.Count;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="RandWeighted{T}"/>.
        /// </summary>
        /// <value>
        /// An <see cref="ICollection{TKey}"/> of the keys in <see cref="RandWeighted{T}"/> ordered from oldest to newest added element.
        /// </value>
        public ICollection<T> Keys => this.items.Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="RandWeighted{T}"/>.
        /// </summary>
        /// <value>
        /// An <see cref="ICollection{TKey}"/> of the values in <see cref="RandWeighted{T}"/> ordered from oldest to newest added element.
        /// </value>
        public ICollection<int> Values => this.items.Values;

        /// <summary>
        /// Gets a value indicating whether the <see cref="RandWeighted{T}"/> is read-only.
        /// </summary>
        /// <value>Always <see langword="true"/>.</value>
        public bool IsReadOnly => this.items.IsReadOnly;

        /// <summary>
        /// Gets a value indicating whether the state of the <see cref="RandWeighted{T}"/> is mutable.
        /// </summary>
        public bool ChangesState => !this.IsReadOnly;

        /// <summary>
        /// Gets a value indicating whether an item can be picked from the <see cref="RandWeighted{T}"/>.
        /// </summary>
        public bool CanPick => this.items.Count > 0;

        /// <summary>
        /// Gets or sets the weight associated with an item contained in the <see cref="RandWeighted{T}"/>.
        /// </summary>
        /// <param name="item">The item associated with the weight to retrieve.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="item"/> does not exist in the <see cref="RandWeighted{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 1.</exception>
        public int this[T item]
        {
            get => this.items[item];
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Weight is less than 1.", nameof(value));

                if (this.ContainsKey(item))
                    this.items[item] = value;
                else
                    this.Add(item, value);
            }
        }

        /// <summary>
        /// Randomly picks an item in the <see cref="RandWeighted{T}"/>.
        /// </summary>
        /// <param name="random">The PRNG used to select the chosen item.</param>
        /// <returns>The selected random value.</returns>
        public T Pick(IRandom random)
        {
            if (this.items.Count == 0)
                throw new InvalidOperationException("No items to pick from");
            if (!this.initialized)
                this.Initialize();
            return this.groups[random.Next(this.groups.Count)].Pick(random);
        }

        /// <summary>
        /// Instantiates a shallow copy of the <see cref="RandWeighted{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="RandWeighted{T}"/>.</returns>
        public RandWeighted<T> CopyState() => new RandWeighted<T>(this);

        /// <summary>
        /// Adds an item with the specified weight to the <see cref="RandWeighted{T}"/>. If the item already exists, adds the weights
        /// together.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="weight">The weight of the item to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="weight"/> is less than 1.</exception>
        public void Add(T item, int weight)
        {
            if (weight < 1)
                throw new ArgumentOutOfRangeException("Weight is less than 1.", nameof(weight));

            if (this.items.ContainsKey(item))
                this[item] += weight;
            else
                this.items.Add(item, weight);
            this.sum += weight;
            this.initialized = false;
        }

        /// <summary>
        /// Removes the specified item from the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if the item was successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
        public bool Remove(T item)
        {
            if (this.items.TryGetValue(item, out int weight))
            {
                this.items.Remove(item);
                this.sum -= weight;
                this.initialized = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the specified item from the <see cref="RandWeighted{T}"/>.
        /// </summary>
        /// <param name="item">The item to attempt to get.</param>
        /// <param name="weight">
        /// If the method succeeded, contains the weight of the specified item; otherwise, contains the default value for
        /// <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="RandWeighted{T}"/> contains the <paramref name="item"/>; otherwise,
        /// /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
        public bool TryGetValue(T item, out int weight)
            => this.items.TryGetValue(item, out weight);

        /// <summary>
        /// Determines whether the <see cref="RandWeighted{T}"/> contains the specified item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="RandWeighted{T}"/> contains the <paramref name="item"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
        public bool ContainsKey(T item)
            => this.items.ContainsKey(item);

        /// <summary>
        /// Removes all elements from the <see cref="RandWeighted{T}"/>.
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
            this.groups.Clear();
            this.sum = 0;
            this.initialized = false;
        }

        /// <summary>
        /// Returns a collection that contains the possible items the <see cref="RandWeighted{T}"/> can select.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate over the possible items.</returns>
        public IEnumerator<KeyValuePair<T, int>> GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        void ICollection<KeyValuePair<T, int>>.Add(KeyValuePair<T, int> item) => this.Add(item.Key, item.Value);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if item was successfully removed from the <see cref="ICollection{T}"/>; otherwise,
        /// <see langword="false"/>. This method also returns false if item is not found in the original <see cref="ICollection{T}"/>.
        /// </returns>
        bool ICollection<KeyValuePair<T, int>>.Remove(KeyValuePair<T, int> item)
        {
            bool ret = ((ICollection<KeyValuePair<T, int>>)this.items).Remove(item);
            this.initialized = !ret;
            return ret;
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        bool ICollection<KeyValuePair<T, int>>.Contains(KeyValuePair<T, int> item)
            => this.items.Contains(item);

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
        void ICollection<KeyValuePair<T, int>>.CopyTo(KeyValuePair<T, int>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T, int>>)this.items).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.items).GetEnumerator();

        /// <summary>
        /// Instantiates a shallow copy of the <see cref="IRandPicker{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="IRandPicker{T}"/>.</returns>
        IRandPicker<T> IRandPicker<T>.CopyState() => this.CopyState();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)this.items.Values).GetEnumerator();

        // Vose's alias method algorithm
        // One-time setup: O(n) best/worst case time complexity
        // Selection: O(1) best/worst case time complexity
        // Splits items (with duplicates) into n groups of two, each of equal total probability, where n is the total number of items.
        private void Initialize()
        {
            if (this.items.Count == 0)
                return;

            if (this.groups.Capacity < this.items.Count)
                this.groups.Capacity = this.items.Count;

            var large = new Queue<(T, long)>();
            var small = new Queue<(T, long)>();

            // First pass: Sort items by weight into two queues
            foreach (KeyValuePair<T, int> pair in this.items)
            {
                (T item, long weight) = (pair.Key, pair.Value * this.items.Count);
                if (weight > this.sum)
                    large.Enqueue((item, weight));
                else
                    small.Enqueue((item, weight));
            }

            // Second pass: Create groups of two from large and small queues
            for (int i = 0; i < this.items.Count; i++)
            {
                (T item, long weight) = small.Dequeue();
                if (weight == this.sum)
                {
                    this.groups.Insert(i, new PresetPicker<T>(item));
                    continue;
                }

                (T item, long weight) alias = large.Dequeue();
                alias.weight -= this.sum - weight;
                if (alias.weight > this.sum)
                    large.Enqueue(alias);
                else if (alias.weight > 0)
                    small.Enqueue(alias);
                this.groups.Insert(i, new RandBiasedCoin<T>(item, alias.item, ((int)weight, this.sum)));
            }

            this.initialized = true;
        }
    }
}