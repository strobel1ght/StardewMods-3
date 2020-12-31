﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Builders;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Runners;
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
            IBuilderFactory factory = this.BuildBuilderFactory();
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


        private IBuilderFactory BuildBuilderFactory()
        {
            var validator = new Validator();
            validator.AddIdentifiableValidation();

            var factory = new BuilderFactory(validator);
            return factory;
        }


        private IResultLogger BuildLogger()
        {
            return new SmapiConsoleLogger(this.Monitor, ModEntry.PrettyPrintIdentifier);
        }


        private IFixtureRunner BuildFixtureRunner()
        {
            return new FixtureRunner();
        }


        private void ListTests(string arg1, string[] arg2)
        {
            // TODO: make conditions show with explanations?
            // TODO: filter via args
            Monitor.Log(string.Join(" ", this._fixtures.Keys));

            foreach (ITestFixture fixture in this._fixtures.Values)
            {
                Monitor.Log(ModEntry.PrettyPrintIdentifier(fixture) + ":");
                if (!fixture.Tests.Any())
                {
                    Monitor.Log("    (no tests)");
                }
                else
                {
                    foreach (IBaseTest test in fixture.Tests)
                    {
                        string s = "    " + ModEntry.PrettyPrintIdentifier(test);
                        if (test is ICasedTest casedTest) s = $"{s} ({casedTest.Cases.Count()} cases)";
                        Monitor.Log(s);
                    }
                }
            }
        }

        private static string PrettyPrintIdentifier(IIdentifiable identifiable)
        {
            return identifiable.LongName == null
                ? identifiable.Key
                : $"{identifiable.LongName} ({identifiable.Key})";
        }


        private void RunTests(string command, string[] args)
        {
            // void RunTest(ITestBuilder test)
            // {
            //     test.RunTest();
            //     test.GetResults().Log(Monitor);
            // }
            //

            if (args.Length == 0)
            {
                foreach (ITestFixture fixture in this._fixtures.Values)
                {
                    IFixtureRunner runner = this.BuildFixtureRunner();
                    ITestResult result = runner.RunFixture(fixture);
                    IResultLogger logger = this.BuildLogger();
                    logger.Log(result);
                }
            }

            throw new NotImplementedException();
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
