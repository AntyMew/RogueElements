﻿// <copyright file="RoomGenCross.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenCross<T> : RoomGen<T> where T : ITiledGenContext
    {
        
        public RandRange MajorWidth;
        public RandRange MajorHeight;
        public RandRange MinorHeight;
        public RandRange MinorWidth;

        [NonSerialized]
        protected int chosenMinorWidth;
        [NonSerialized]
        protected int chosenMinorHeight;
        [NonSerialized]
        protected int chosenOffsetX;
        [NonSerialized]
        protected int chosenOffsetY;

        public RoomGenCross() { }

        public RoomGenCross(RandRange majorWidth, RandRange majorHeight, RandRange minorHeight, RandRange minorWidth)
        {
            MajorWidth = majorWidth;
            MajorHeight = majorHeight;
            MinorWidth = minorWidth;
            MinorHeight = minorHeight;
        }
        protected RoomGenCross(RoomGenCross<T> other)
        {
            MajorWidth = other.MajorWidth;
            MajorHeight = other.MajorHeight;
            MinorWidth = other.MinorWidth;
            MinorHeight = other.MinorHeight;
        }
        public override RoomGen<T> Copy() { return new RoomGenCross<T>(this); }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(MajorWidth.Pick(rand), MajorHeight.Pick(rand));
        }


        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            chosenMinorWidth = Math.Min(Draw.Width, MinorWidth.Pick(rand));
            chosenMinorHeight = Math.Min(Draw.Height, MinorHeight.Pick(rand));

            chosenOffsetX = rand.Next(Draw.Width - chosenMinorWidth + 1);
            chosenOffsetY = rand.Next(Draw.Height - chosenMinorHeight + 1);


            for (int jj = chosenOffsetX; jj < chosenOffsetX + chosenMinorWidth; jj++)
            {
                fulfillableBorder[(int)Dir4.Up][jj] = true;
                fulfillableBorder[(int)Dir4.Down][jj] = true;
            }
            for (int jj = chosenOffsetY; jj < chosenOffsetY + chosenMinorHeight; jj++)
            {
                fulfillableBorder[(int)Dir4.Left][jj] = true;
                fulfillableBorder[(int)Dir4.Right][jj] = true;
            }
        }

        public override void DrawOnMap(T map)
        {
            Loc size1 = new Loc(Draw.Width, chosenMinorHeight);
            Loc size2 = new Loc(chosenMinorWidth, Draw.Height);

            Loc start1 = new Loc(Draw.X, Draw.Y + chosenOffsetY);
            Loc start2 = new Loc(Draw.X + chosenOffsetX, Draw.Y);
            
            for (int x = 0; x < size1.X; x++)
            {
                for (int y = 0; y < size1.Y; y++)
                    map.SetTile(new Loc(start1.X + x, start1.Y + y), map.RoomTerrain.Copy());
            }
            GenContextDebug.DebugProgress("First Rect");
            for (int x = 0; x < size2.X; x++)
            {
                for (int y = 0; y < size2.Y; y++)
                    map.SetTile(new Loc(start2.X + x, start2.Y + y), map.RoomTerrain.Copy());
            }
            GenContextDebug.DebugProgress("Second Rect");

            //hall restrictions
            SetRoomBorders(map);
        }
    }
}
