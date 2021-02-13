using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Phrasefable.StardewMods.AggressiveAcorns.Test
{
    [TestFixture]
    public class ToolActionTests
    {
        private readonly GameLocation _dummyLocation = new DummyLocation();


        [TestCase(false, true)] // Default (vanilla) - weapons effect trees
        [TestCase(true, false)] // Modded - blanket-prevent melee on trees
        public void MeleeWeaponDependsOnConfig(bool configValue, bool effectsTree)
        {
            AggressiveAcorns.Config = new ConfigAdaptor(new ModConfig {PreventScythe = configValue});
            var shim = new ToolActionShim(effectsTree);
            var tree = new AggressiveTree(shim.Shim);

            Tool tool = ToolFactory.getToolFromDescription(ToolFactory.meleeWeapon, Tool.stone);
            bool result = tree.performToolAction(tool, 0, Vector2.Zero, this._dummyLocation);

            shim.Called.Should().Be(!configValue);
            result.Should().Be(effectsTree);
        }


        [Test]
        public void MeleeDelegatesCallAndUsesResultByDefault()
        {
            this.AssertDelegatesCallAndUsesResult(ToolFactory.meleeWeapon);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void OtherToolsDelegateCallAndUseResult(bool configValue)
        {
            AggressiveAcorns.Config = new ConfigAdaptor(new ModConfig {PreventScythe = configValue});

            this.AssertDelegatesCallAndUsesResult(ToolFactory.axe);
            this.AssertDelegatesCallAndUsesResult(ToolFactory.pickAxe);
            this.AssertDelegatesCallAndUsesResult(ToolFactory.hoe);
        }


        private void AssertDelegatesCallAndUsesResult(byte toolType)
        {
            foreach (bool fakeVanillaOutput in new[] {true, false})
            {
                // Arrange
                var shim = new ToolActionShim(fakeVanillaOutput);
                var tree = new AggressiveTree(shim.Shim);
                Tool tool = ToolFactory.getToolFromDescription(toolType, Tool.stone);

                // Act
                bool result = tree.performToolAction(tool, 0, Vector2.Zero, _dummyLocation);

                // Assert
                shim.Called.Should().BeTrue();
                result.Should().Be(fakeVanillaOutput);
            }
        }

        // ==================== Utility Classes ====================

        private class ToolActionShim
        {
            public bool Called { get; private set; }

            public readonly Func<Tool, int, Vector2, GameLocation, bool> Shim;

            public ToolActionShim(bool returnValue)
            {
                this.Shim = (t, i, v, l) =>
                {
                    Called = true;
                    return returnValue;
                };
            }
        }

        [SuppressMessage("ReSharper", "EmptyConstructor")]
        [SuppressMessage("ReSharper", "RedundantBaseConstructorCall")]
        private class DummyLocation : GameLocation
        {
            public DummyLocation() : base() { }
            protected override void initNetFields() { }
        }
    }
}