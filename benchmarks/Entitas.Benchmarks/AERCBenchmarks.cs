#nullable disable

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

// |     Method |      Mean |     Error |    StdDev | Rank | Allocated |
// |----------- |----------:|----------:|----------:|-----:|----------:|
// | UnsafeAERC |  1.153 ns | 0.0035 ns | 0.0033 ns |    1 |         - |
// |   SafeAERC | 18.992 ns | 0.0751 ns | 0.0665 ns |    2 |         - |

namespace Entitas.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AERCBenchmarks
    {
        SafeAERC _safeAerc;
        UnsafeAERC _unsafeAerc;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _safeAerc = new SafeAERC(null);
            _unsafeAerc = new UnsafeAERC();
        }

        [Benchmark]
        public void SafeAERC()
        {
            _safeAerc.Retain(this);
            _safeAerc.Release(this);
        }

        [Benchmark]
        public void UnsafeAERC()
        {
            _unsafeAerc.Retain(this);
            _unsafeAerc.Release(this);
        }
    }
}
