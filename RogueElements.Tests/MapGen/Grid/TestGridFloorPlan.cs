// <copyright file="TestGridFloorPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    public class TestGridFloorPlan : GridPlan
    {
        public List<GridRoomPlan> PublicArrayRooms => this.ArrayRooms;

        public int[][] PublicRooms => this.Rooms;

        public GridHallPlan[][] PublicVHalls => this.VHalls;

        public GridHallPlan[][] PublicHHalls => this.HHalls;

        public static void CompareFloorPlans(TestGridFloorPlan floorPlan, TestGridFloorPlan compareFloorPlan)
        {
            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(compareFloorPlan.RoomCount));
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                GridRoomPlan comparePlan = compareFloorPlan.GetRoomPlan(ii);
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Bounds, Is.EqualTo(comparePlan.Bounds));
            }

            // check positions
            Assert.That(floorPlan.PublicRooms, Is.EqualTo(compareFloorPlan.PublicRooms));
            Assert.That(floorPlan.PublicVHalls.Length, Is.EqualTo(compareFloorPlan.PublicVHalls.Length));
            for (int xx = 0; xx < floorPlan.PublicVHalls.Length; xx++)
            {
                Assert.That(floorPlan.PublicVHalls[xx].Length, Is.EqualTo(compareFloorPlan.PublicVHalls[xx].Length));
                for (int yy = 0; yy < floorPlan.PublicVHalls[xx].Length; yy++)
                    Assert.That(floorPlan.PublicVHalls[xx][yy].Gens, Is.EqualTo(compareFloorPlan.PublicVHalls[xx][yy].Gens));
            }

            Assert.That(floorPlan.PublicHHalls.Length, Is.EqualTo(compareFloorPlan.PublicHHalls.Length));
            for (int xx = 0; xx < floorPlan.PublicHHalls.Length; xx++)
            {
                Assert.That(floorPlan.PublicHHalls[xx].Length, Is.EqualTo(compareFloorPlan.PublicHHalls[xx].Length));
                for (int yy = 0; yy < floorPlan.PublicVHalls[xx].Length; yy++)
                    Assert.That(floorPlan.PublicHHalls[xx][yy].Gens, Is.EqualTo(compareFloorPlan.PublicHHalls[xx][yy].Gens));
            }
        }

        public static TestGridFloorPlan InitGridToContext(string[] inGrid)
        {
            return InitGridToContext(inGrid, 0, 0);
        }

        public static TestGridFloorPlan InitGridToContext(string[] inGrid, int widthPerCell, int heightPerCell)
        {
            // transposes
            if (inGrid.Length % 2 == 0 || inGrid[0].Length % 2 == 0)
                throw new ArgumentException("Bad input grid!");
            var floorPlan = new TestGridFloorPlan();
            floorPlan.InitSize((inGrid[0].Length / 2) + 1, (inGrid.Length / 2) + 1, widthPerCell, heightPerCell);
            GridRoomPlan[] addedRooms = new GridRoomPlan[26];

            for (int xx = 0; xx < inGrid[0].Length; xx++)
            {
                for (int yy = 0; yy < inGrid.Length; yy++)
                {
                    char val = inGrid[yy][xx];
                    int x = xx / 2;
                    int y = yy / 2;

                    // rooms
                    if (xx % 2 == 0 && yy % 2 == 0)
                    {
                        if (val >= 'A' && val <= 'Z')
                        {
                            floorPlan.Rooms[x][y] = val - 'A';
                            if (addedRooms[val - 'A'] == null)
                                addedRooms[val - 'A'] = new GridRoomPlan(new Rect(x, y, 1, 1), new TestGridRoomGen(val));
                            addedRooms[val - 'A'].Bounds = Rect.IncludeLoc(addedRooms[val - 'A'].Bounds, new Loc(x, y));
                        }
                        else if (val == '0')
                        {
                            floorPlan.Rooms[x][y] = -1;
                        }
                        else
                        {
                            throw new ArgumentException($"Bad input grid val at room {x},{y}!");
                        }
                    }
                    else if (xx % 2 == 0 && yy % 2 == 1)
                    {
                        // vhalls
                        if (val == '#')
                            floorPlan.VHalls[x][y].SetGen(new TestGridRoomGen());
                        else if (val == '.')
                            floorPlan.VHalls[x][y].SetGen(null);
                        else
                            throw new ArgumentException($"Bad input grid val at vertical hall {x},{y}!");
                    }
                    else if (xx % 2 == 1 && yy % 2 == 0)
                    {
                        // hhalls
                        if (val == '#')
                            floorPlan.HHalls[x][y].SetGen(new TestGridRoomGen());
                        else if (val == '.')
                            floorPlan.HHalls[x][y].SetGen(null);
                        else
                            throw new ArgumentException($"Bad input grid val at horizontal hall {x},{y}!");
                    }
                    else if (xx % 2 == 1 && yy % 2 == 1)
                    {
                        // blank
                        if (val != ' ')
                            throw new ArgumentException("Bad input grid val at blank zone!");
                    }
                }
            }

            for (int ii = 0; ii < 26; ii++)
            {
                if (addedRooms[ii] != null)
                    floorPlan.ArrayRooms.Add(addedRooms[ii]);
            }

            return floorPlan;
        }
    }
}