using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

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
