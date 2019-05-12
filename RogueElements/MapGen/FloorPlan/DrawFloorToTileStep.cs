﻿// <copyright file="DrawFloorToTileStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DrawFloorToTileStep<T> : GenStep<T> where T : class, IFloorPlanGenContext
    {
        public int Padding;

        public DrawFloorToTileStep() { }

        public DrawFloorToTileStep(int padding) { Padding = padding; }

        public override void Apply(T map)
        {
            // draw on map
            map.CreateNew(map.RoomPlan.DrawRect.Width + 2 * Padding,
                map.RoomPlan.DrawRect.Height + 2 * Padding);
            for (int ii = 0; ii < map.Width; ii++)
            {
                for (int jj = 0; jj < map.Height; jj++)
                    map.SetTile(new Loc(ii,jj), map.WallTerrain.Copy());
            }
            map.RoomPlan.MoveStart(new Loc(Padding));
            GenContextDebug.DebugProgress("Moved Floor");
            map.RoomPlan.DrawOnMap(map);
        }

    }
}
