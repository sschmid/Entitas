using System.Diagnostics;

public static class PerformanceTestRunner {

    static readonly Stopwatch _stopwatch;

    static PerformanceTestRunner() {
        _stopwatch = new Stopwatch();
    }

    public static long Run(IPerformanceTest test) {
        test.Before();
        _stopwatch.Reset();
        _stopwatch.Start();
        test.Run();
        _stopwatch.Stop();
        return _stopwatch.ElapsedMilliseconds;
    }
}
