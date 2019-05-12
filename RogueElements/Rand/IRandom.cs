﻿// <copyright file="IRandom.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Diagnostics.Contracts;


namespace RogueElements
{
    public interface IRandom
    {

        ulong FirstSeed { get; }


        ulong NextUInt64();


        int Next();

        int Next(int minValue, int maxValue);


        int Next(int maxValue);


        double NextDouble();
    }

}
