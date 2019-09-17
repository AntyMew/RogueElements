// <copyright file="SpawnListExceptionTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnListExceptionTest
    {
        [Test]
        public void SpawnListEmptyChoose()
        {
            // choose when empty
            var picker = new SpawnList<string>();
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(picker.CanPick, Is.EqualTo(false));
            Assert.Throws<InvalidOperationException>(() => { picker.Pick(testRand.Object); });
        }

        [Test]
        public void SpawnListAddZero()
        {
            // choose when all 0's
            var picker = new SpawnList<string>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { picker.Add("apple", 0); });
        }

        [Test]
        public void SpawnListAddNegative()
        {
            // add negative
            var picker = new SpawnList<string>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { picker.Add("apple", -1); });
        }

        [Test]
        public void SpawnListSetNegative()
        {
            // set negative
            SpawnList<string> picker = new SpawnList<string> { { "apple", 1 } };
            Assert.Throws<ArgumentOutOfRangeException>(() => { picker["apple"] = -1; });
        }
    }
}