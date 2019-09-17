// <copyright file="RandWeightedExceptionTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RandWeightedExceptionTest
    {
        [Test]
        public void RandWeightedEmptyChoose()
        {
            // choose when empty
            var picker = new RandWeighted<string>();
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(picker.CanPick, Is.EqualTo(false));
            Assert.Throws<InvalidOperationException>(() => { picker.Pick(testRand.Object); });
        }

        [Test]
        public void RandWeightedAddZero()
        {
            // choose when all 0's
            var picker = new RandWeighted<string>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { picker.Add("apple", 0); });
        }

        [Test]
        public void RandWeightedAddNegative()
        {
            // add negative
            var picker = new RandWeighted<string>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { picker.Add("apple", -1); });
        }

        [Test]
        public void RandWeightedSetNegative()
        {
            // set negative
            RandWeighted<string> picker = new RandWeighted<string> { { "apple", 1 } };
            Assert.Throws<ArgumentOutOfRangeException>(() => { picker["apple"] = -1; });
        }
    }
}