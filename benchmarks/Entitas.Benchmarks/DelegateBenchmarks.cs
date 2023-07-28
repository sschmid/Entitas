#nullable disable

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

// |         Method |      Mean |     Error |    StdDev | Rank |   Gen0 | Allocated |
// |--------------- |----------:|----------:|----------:|-----:|-------:|----------:|
// |       Delegate |  4.674 ns | 0.0105 ns | 0.0093 ns |    1 | 0.0115 |      24 B |
// | InstanceMethod | 10.458 ns | 0.0646 ns | 0.0604 ns |    2 | 0.0421 |      88 B |

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
}
