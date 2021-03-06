﻿// <copyright file="EraseIsolatedStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class EraseIsolatedStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        public EraseIsolatedStep()
        {
        }

        public EraseIsolatedStep(ITile terrain)
        {
            this.Terrain = terrain;
        }

        public ITile Terrain { get; set; }

        public override void Apply(T map)
        {
            bool[][] connectionGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                connectionGrid[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                    connectionGrid[xx][yy] = false;
            }

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    // upon detecting an unmarked room area, fill with connected marks
                    if (map.GetTile(new Loc(xx, yy)).TileEquivalent(map.RoomTerrain) && !connectionGrid[xx][yy])
                    {
                        Grid.FloodFill(
                            new Rect(0, 0, map.Width, map.Height),
                            (Loc testLoc) =>
                            {
                                bool blocked = map.TileBlocked(testLoc);
                                blocked &= !map.GetTile(testLoc).TileEquivalent(this.Terrain);
                                return connectionGrid[testLoc.X][testLoc.Y] || blocked;
                            },
                            (Loc testLoc) => true,
                            (Loc fillLoc) => connectionGrid[fillLoc.X][fillLoc.Y] = true,
                            new Loc(xx, yy));
                    }
                }
            }

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (map.GetTile(new Loc(xx, yy)).TileEquivalent(this.Terrain) && !connectionGrid[xx][yy])
                        map.SetTile(new Loc(xx, yy), map.WallTerrain.Copy());
                }
            }
        }
    }
}
