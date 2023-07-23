#nullable disable

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

// |    Method |     Mean |     Error |    StdDev | Rank |   Gen0 | Allocated |
// |---------- |---------:|----------:|----------:|-----:|-------:|----------:|
// |    Manual | 3.392 ns | 0.0159 ns | 0.0149 ns |    1 | 0.0115 |      24 B |
// |   Generic | 8.100 ns | 0.0564 ns | 0.0500 ns |    2 | 0.0115 |      24 B |
// | Activator | 9.741 ns | 0.0352 ns | 0.0312 ns |    3 | 0.0115 |      24 B |

namespace Entitas.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class CreateComponentBenchmarks
    {
        Entity _entity;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var context = new Context<Entity>(1, () => new Entity());
            _entity = context.CreateEntity();
        }

        [Benchmark]
        public MovableComponent Activator()
        {
            return (MovableComponent)_entity.CreateComponent(0, typeof(MovableComponent));
        }

        [Benchmark]
        public MovableComponent Generic()
        {
            return _entity.CreateComponent<MovableComponent>(0);
        }

        [Benchmark]
        public MovableComponent Manual()
        {
            var componentPool = _entity.GetComponentPool(0);
            return componentPool.Count > 0
                ? (MovableComponent)componentPool.Pop()
                : new MovableComponent();
        }
    }

    public sealed class MovableComponent : IComponent { }
}
