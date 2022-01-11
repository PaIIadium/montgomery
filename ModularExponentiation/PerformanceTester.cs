namespace ModularExponentiation
{
    using System;
    using System.Diagnostics;

    public class PerformanceTester
    {
        public static float Run(Action action, int iterationsCont)
        {
            var nsPerTick = 1000_000_1000L / (float)Stopwatch.Frequency;
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterationsCont; i++)
            {
                action();
            }
            stopwatch.Stop();
            var elapsedTicks = stopwatch.ElapsedTicks;
            return elapsedTicks * nsPerTick / iterationsCont;
        }
    }
}