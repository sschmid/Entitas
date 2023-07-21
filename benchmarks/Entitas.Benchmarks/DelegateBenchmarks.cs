#nullable disable

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Entitas.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DelegateBenchmarks
    {
        static readonly Func<int, int> TimesTwoDelegate = a => a + a;

        int TimesTwo(int a) => a + a;

        [Benchmark]
        public void InstanceMethod()
        {
            new MyClass(TimesTwo).Invoke(1);
        }

        [Benchmark]
        public void Delegate()
        {
            new MyClass(TimesTwoDelegate).Invoke(1);
        }
    }

    class MyClass
    {
        readonly Func<int, int> _method;

        public MyClass(Func<int, int> method)
        {
            _method = method;
        }

        public int Invoke(int a) => _method(a);
    }
}
