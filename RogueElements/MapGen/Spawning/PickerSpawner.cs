// <copyright file="PickerSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Geenrates spawnables from a specifically defined IMultiRandPicker.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class PickerSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        public PickerSpawner()
        {
        }

        public PickerSpawner(IRandPicker<IEnumerable<TSpawnable>> picker)
        {
            this.Picker = picker;
        }

        public IRandPicker<IEnumerable<TSpawnable>> Picker { get; set; }

        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            if (this.Picker is null)
                return new List<TSpawnable>();
            IRandPicker<IEnumerable<TSpawnable>> picker = this.Picker;
            if (picker.ChangesState)
                picker = picker.CopyState();
            var copyResults = new List<TSpawnable>();
            foreach (TSpawnable result in picker.Pick(map.Rand))
                copyResults.Add((TSpawnable)result.Copy());
            return copyResults;
        }
    }
}
