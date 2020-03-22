using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinder;

namespace Tests
{
    [TestClass]
    public class PathFinderTests
    {

        public double TestSpeedPerformance(Action action, int retry)
        {
            List<TimeSpan> times = new List<TimeSpan>();
            for (int i = 0; i < retry; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                action();
                stopwatch.Stop();
                times.Add(stopwatch.Elapsed);
            }

            return times.Sum(t => t.TotalMilliseconds) / times.Count;
        }

        [TestMethod]
        public void OneRootPathParseDirectories()
        {
            SearchEngine engine = new SearchEngine();
            Action action1 = () =>
            {
                var results = new List<string>();
                engine.ResolveSelectionTree();

                foreach (var softwareRelease in engine.SelectionThree)
                {
                    foreach (var softwareReleaseBuild in softwareRelease.Builds)
                    {
                        results.Add($"{softwareRelease.Name} {softwareReleaseBuild.Name}");
                    }
                }
            };

            Action action3 = () =>
            {
                var builds = engine.SelectionThree.First().Builds;
                var brands = builds.First().Brands;
                var oems = brands.First().Oems;
                foreach (var oem in oems)
                {
                    Console.WriteLine(oem.Name);
                }
            };
            

            var meanTime1 = TestSpeedPerformance(action1, 1);
            Console.WriteLine(meanTime1);


            var meanTime3 = TestSpeedPerformance(action3, 10);
            Console.WriteLine(meanTime3);

            Console.WriteLine(engine.SelectionThree.First().Builds.First().Brands.First().Oems.First().SetupPath);

        }
    }
}
