using System;
using System.Linq;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Phrasefable.StardewMods.AggressiveAcorns.Test
{
    [TestFixture]
    public class SpreadSeedOffsetsTests
    {
        private static readonly int MaxAttempts = 999;


        [OneTimeSetUp]
        public void SetUp()
        {
            AggressiveAcorns.Config = new ConfigAdaptor(new ModConfig());
        }


        [TestCase(-3)]
        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestHitsX(int offset)
        {
            Find(v => ((int) v.X) == offset);
        }


        [TestCase(-3)]
        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TestHitsY(int offset)
        {
            Find(v => ((int) v.Y) == offset);
        }


        private void Find(Func<Vector2, bool> pred)
        {
            var i = 0;
            while (i < SpreadSeedOffsetsTests.MaxAttempts)
            {
                i++;
                if (TreeUtils.GetSpreadOffsets().Any(pred))
                {
                    Assert.Pass($"Found in {i} attempts.");
                }
            }

            Assert.Fail($"Not found after {i} attempts");
        }
    }
}