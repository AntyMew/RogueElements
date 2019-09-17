// <copyright file="RoomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class RoomSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        protected RoomSpawnStep()
            : base()
        {
        }

        protected RoomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
        }

        public virtual void SpawnRandInCandRooms(TGenContext map, SpawnList<RoomHallIndex> spawningRooms, List<TSpawnable> spawns, int successPercent)
        {
            while (spawningRooms.Count > 0 && spawns.Count > 0)
            {
                RoomHallIndex roomIndex = spawningRooms.Pick(map.Rand);

                // try to spawn the item
                if (this.SpawnInRoom(map, roomIndex, spawns[spawns.Count - 1]))
                {
                    GenContextDebug.DebugProgress("Placed Object");

                    // remove the item spawn
                    spawns.RemoveAt(spawns.Count - 1);

                    if (successPercent <= 0)
                    {
                        spawningRooms.Remove(roomIndex);
                    }
                    else
                    {
                        int newRate = Math.Max(1, spawningRooms[roomIndex] * successPercent / 100);
                        spawningRooms[roomIndex] = newRate;
                    }
                }
                else
                {
                    spawningRooms.Remove(roomIndex);
                }
            }
        }

        public virtual bool SpawnInRoom(TGenContext map, RoomHallIndex roomIndex, TSpawnable spawn)
        {
            IRoomGen room = map.RoomPlan.GetRoomHall(roomIndex).RoomGen;
            List<Loc> freeTiles = map.GetFreeTiles(room.Draw);

            if (freeTiles.Count > 0)
            {
                int randIndex = map.Rand.Next(freeTiles.Count);
                map.PlaceItem(freeTiles[randIndex], spawn);
                return true;
            }

            return false;
        }
    }
}
