﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Tests;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest
{
    [UsedImplicitly]
    public class ModEntry : Mod
    {
        private readonly IDictionary<string, ITestFixture> _fixtures = new SortedDictionary<string, ITestFixture>();


        public override void Entry(IModHelper helper)
        {
            SetUpTests();

            // TODO implement test runner
            // var desc = new StringBuilder();
            // desc.AppendLine("");
            // desc.AppendLine("Usage:");
            // desc.AppendLine("  run_tests [<test> ...]");
            // desc.AppendLine("Runs specified tests. If none specified, runs all tests.");
            // desc.Append("Use command `list_tests` to determine test names.");

            // helper.ConsoleCommands.Add("run_tests", desc.ToString(), this.RunTests);
            helper.ConsoleCommands.Add("list_tests", "Lists test fixtures.", ListTests);
        }


        private void SetUpTests()
        {
            IBuilderFactory factory = GetBuilderFactory();
            foreach (ITestFixtureDefinition fixtureDefinition in new ITestFixtureDefinition[]
            {
                new Seed_Tests(),
                new TreeUtils_ExperiencesWinter_Test()
            })
            {
                ITestFixture fixture = fixtureDefinition.GetFixture(factory);
                this._fixtures.Add(fixture.Key, fixture);
            }
        }


        private IBuilderFactory GetBuilderFactory()
        {
            throw new NotImplementedException();
        }


        private void ListTests(string arg1, string[] arg2)
        {
            throw new NotImplementedException();
        }


        private void RunTests(string command, string[] args)
        {
            throw new NotImplementedException();
            // void RunTest(ITestBuilder test)
            // {
            //     test.RunTest();
            //     test.GetResults().Log(Monitor);
            // }
            //
            // if (args.Length == 0)
            // {
            //     foreach (ITestBuilder test in this._tests.Values)
            //     {
            //         RunTest(test);
            //     }
            // }
            //
            // List<string> badKeys = args
            //     .Where(arg => !this._tests.ContainsKey(arg))
            //     .ToList();
            //
            // if (badKeys.Any())
            // {
            //     Monitor.Log(
            //         $"Test run aborted. Invalid test name(s) present: {string.Join(" ", badKeys)}",
            //         LogLevel.Error
            //     );
            //     Monitor.Log($"Available tests: {this.TestNames}", LogLevel.Error);
            //     return;
            // }
            //
            // foreach (string testId in args)
            // {
            //     RunTest(this._tests[testId]);
            // }
        }
    }
}
