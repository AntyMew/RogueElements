// <copyright file="RandBiasedCoin.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A container which can randomly pick one of two items, with a probabilistic bias.
    /// </summary>
    /// <typeparam name="T">The type of the item to pick.</typeparam>
    public class RandBiasedCoin<T> : IRandPicker<T>
    {
        private readonly T first;
        private readonly T second;
        private readonly (int numerator, int denominator) probability;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBiasedCoin{T}"/> class.
        /// </summary>
        /// <param name="first">The first item which can be picked.</param>
        /// <param name="second">The second item which can be picked.</param>
        /// <param name="probability">A real number representing the probability of picking <paramref name="first"/>.</param>
        /// <exception cref="DivideByZeroException">Thrown if <paramref name="probability.denominator"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="probability"/> is negative.</exception>
        public RandBiasedCoin(T first, T second, (int numerator, int denominator) probability)
        {
            if (probability.numerator < 0 && probability.denominator < 0)
            {
                this.probability.numerator = -probability.numerator;
                this.probability.denominator = -probability.denominator;
            }
            else if (probability.numerator < 0 || probability.denominator < 0)
            {
                throw new ArgumentOutOfRangeException("Probability must be positive.", nameof(probability));
            }

            if (probability.denominator == 0)
                throw new DivideByZeroException();

            this.first = first;
            this.second = second;
            this.probability = probability;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBiasedCoin{T}"/> class as a copy of another instance.
        /// </summary>
        /// <param name="other">The <see cref="RandBiasedCoin{T}"/> instance to copy.</param>
        protected RandBiasedCoin(RandBiasedCoin<T> other)
            : this(other.first, other.second, other.probability)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this object changes after a call to <see cref="Pick"/>.
        /// </summary>
        public bool ChangesState => false;

        /// <summary>
        /// Gets a value indicating whether this instance is in a state where <see cref="Pick"/> can be called without throwing an
        /// exception.
        /// </summary>
        /// <value>Always <see langword="true"/>.</value>
        public bool CanPick => true;

        /// <summary>
        /// Randomly picks one of the two items in the <see cref="RandBiasedCoin{T}"/>.
        /// </summary>
        /// <param name="random">The PRNG used to select the chosen item.</param>
        /// <returns>The selected random value.</returns>
        public T Pick(IRandom random)
        {
            return random.Next(this.probability.denominator) < this.probability.numerator ? this.first : this.second;
        }

        /// <summary>
        /// Instantiates a shallow copy of the <see cref="RandBiasedCoin{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="RandBiasedCoin{T}"/>.</returns>
        public RandBiasedCoin<T> CopyState() => new RandBiasedCoin<T>(this);

        /// <summary>
        /// Instantiates a shallow copy of the <see cref="IRandPicker{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="IRandPicker{T}"/>.</returns>
        IRandPicker<T> IRandPicker<T>.CopyState() => this.CopyState();
    }
}