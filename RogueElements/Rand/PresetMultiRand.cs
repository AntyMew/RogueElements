﻿// <copyright file="PresetMultiRand.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a list of items predefined by the user.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PresetMultiRand<T> : IMultiRandPicker<T>
    {
        public PresetMultiRand()
        {
            this.ToSpawn = new List<T>();
        }

        public PresetMultiRand(params T[] toSpawn)
        {
            this.ToSpawn = new List<T>(toSpawn);
        }

        public PresetMultiRand(List<T> toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        protected PresetMultiRand(PresetMultiRand<T> other)
        {
            this.ToSpawn = new List<T>(other.ToSpawn);
        }

        public List<T> ToSpawn { get; }

        public bool ChangesState => false;

        public bool CanPick => this.ToSpawn != null;

        public IMultiRandPicker<T> CopyState() => new PresetMultiRand<T>(this);

        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            result.AddRange(this.ToSpawn);
            return result;
        }
    }
}
