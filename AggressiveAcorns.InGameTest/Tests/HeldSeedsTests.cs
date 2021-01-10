using Phrasefable.StardewMods.StarUnit.Framework;
using Phrasefable.StardewMods.StarUnit.Framework.Builders;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;
using Phrasefable.StardewMods.StarUnit.Framework.Results;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests
{
    internal class HeldSeedsTests
    {
        private readonly ITestDefinitionFactory _factory;

        private MutableConfigAdaptor _config;


        public HeldSeedsTests(ITestDefinitionFactory factory)
        {
            this._factory = factory;
        }


        public ITraversable Build()
        {
            ITestFixtureBuilder fixtureBuilder = _factory.CreateFixtureBuilder();

            fixtureBuilder.Key = "held_seeds";

            fixtureBuilder.AddCondition(this._factory.Conditions.WorldReady);

            /* Note warps are not synchronous - printing the players position before and after the warp
             * statement does not show a difference. Still seems to work (the locations used in this test are
             * always loaded??), so have not bothered to make the test framework asynchronous.
             * */
            fixtureBuilder.BeforeAll = () => Game1.player.warpFarmer(Utils.WarpFarm);
            fixtureBuilder.BeforeEach = () =>
            {
                this._config = new MutableConfigAdaptor {DailySpreadChance = 0.0};
                AggressiveAcorns.Config = this._config;
            };

            fixtureBuilder.AddChild(this.BuildTest_HeldSeed());
            fixtureBuilder.AddChild(this.BuildTest_HeldSeed_Override());

            return fixtureBuilder.Build();
        }


        private ITestResult CheckTreeHasSeedAfterUpdate(Tree tree, bool expectSeed)
        {
            // Act
            tree.Update();

            // Assert
            return tree.hasSeed.Value == expectSeed
                ? this._factory.BuildTestResult(Status.Pass, null)
                : this._factory.BuildTestResult(Status.Fail, expectSeed ? "Seed expected" : "Seed not expected");
        }


        // ========== Held seed, varying config chance ================================================================

        private ITraversable BuildTest_HeldSeed()
        {
            ICasedTestBuilder<(double SeedChance, bool ExpectSeed)> testBuilder =
                _factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_holds_seeds";
            testBuilder.TestMethod = this.Test_HeldSeed;
            testBuilder.KeyGenerator = @case => $"chance_{@case.SeedChance * 100}";
            testBuilder.AddCases(
                (SeedChance: 0.0, ExpectSeed: false),
                (SeedChance: 1.0, ExpectSeed: true)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed((double, bool) @params)
        {
            (double seedChance, bool expectSeed) = @params;

            // Arrange
            this._config.DailySeedChance = seedChance;

            // Act, assert
            return CheckTreeHasSeedAfterUpdate(Utils.GetFarmTreeLonely(), expectSeed);
        }


        // ========== Held seed, overriding random function ============================================================

        private ITraversable BuildTest_HeldSeed_Override()
        {
            ICasedTestBuilder<(double Chance, bool ExpectSeed)> testBuilder =
                _factory.CreateCasedTestBuilder<(double, bool)>();

            testBuilder.Key = "tree_holds_seeds_override_random";
            testBuilder.TestMethod = this.Test_HeldSeed_Override;
            testBuilder.KeyGenerator = @case => $"chance_{@case.Chance * 100}_random_always_{@case.ExpectSeed}";
            testBuilder.AddCases(
                (Chance: 0.0, ExpectSeed: true),
                (Chance: 0.5, ExpectSeed: true),
                (Chance: 1.0, ExpectSeed: true),
                (Chance: 0.0, ExpectSeed: false),
                (Chance: 0.5, ExpectSeed: false),
                (Chance: 1.0, ExpectSeed: false)
            );
            return testBuilder.Build();
        }


        private ITestResult Test_HeldSeed_Override((double Chance, bool ExpectSeed) @params)
        {
            (double configChance, bool expectSeed) = @params;

            // Arrange
            this._config.DailySeedChance = configChance;
            this._config.SeedRoller = () => expectSeed;

            // Act, assert
            return this.CheckTreeHasSeedAfterUpdate(Utils.GetFarmTreeLonely(), expectSeed);
        }
    }
}