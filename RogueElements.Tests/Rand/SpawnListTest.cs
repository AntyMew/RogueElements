// <copyright file="SpawnListTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnListTest
    {
        private SpawnList<string> picker;

        [SetUp]
        public void SpawnListSetUp()
        {
            this.picker = new SpawnList<string>
            {
                { "apple", 10 },
                { "orange", 20 },
                { "banana", 30 },
            };
        }

        [Test]
        public void SpawnListCanPick()
        {
            Assert.That(this.picker.CanPick);
        }

        [Test]
        [TestCase("apple", 10)]
        [TestCase("orange", 20)]
        [TestCase("banana", 30)]
        public void SpawnListGetWeight(string spawn, int weight)
        {
            // get spawn rate
            Assert.That(this.picker[spawn], Is.EqualTo(weight));
        }

        [Test]
        [TestCase(5)]
        public void SpawnListSetWeight(int weight)
        {
            // set spawn rate
            this.picker["apple"] = weight;
            Assert.That(this.picker["apple"], Is.EqualTo(weight));
        }

        [Test]
        public void SpawnListAdd()
        {
            // add a spawn
            this.picker.Add("watermelon", 15);
            Assert.That(this.picker.TryGetValue("watermelon", out int weight));
            Assert.That(weight, Is.EqualTo(15));
            Assert.That(this.picker.Count, Is.EqualTo(4));
        }

        [Test]
        public void SpawnListAddExisting()
        {
            // add an existing spawn
            this.picker.Add("apple", 15);
            Assert.That(this.picker.TryGetValue("apple", out int weight));
            Assert.That(weight, Is.EqualTo(25));
            Assert.That(this.picker.Count, Is.EqualTo(3));
        }

        [Test]
        public void SpawnListRemove()
        {
            // remove a spawn
            this.picker.Remove("apple");
            Assert.That(this.picker.TryGetValue("apple", out int _), Is.False);
            Assert.That(this.picker.Count, Is.EqualTo(2));
        }

        [Test]
        public void SpawnListClear()
        {
            // remove all spawn
            this.picker.Clear();
            Assert.That(this.picker.Count, Is.Zero);
            Assert.That(this.picker.CanPick, Is.False);
        }

        [Test]
        [TestCase("apple", 10)]
        [TestCase("orange", 20)]
        [TestCase("banana", 30)]
        public void SpawnListGetWeights(string item, int weight)
        {
            // check all spawns
            Assert.That(this.picker[item], Is.EqualTo(weight));
        }
    }
}