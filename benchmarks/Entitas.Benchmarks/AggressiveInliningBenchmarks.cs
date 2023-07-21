using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Entitas.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AggressiveInliningBenchmarks
    {
        readonly EntityNoOptions _entityNoOptions;
        readonly EntityAggressiveInlining _entityAggressiveInlining;

        public AggressiveInliningBenchmarks()
        {
            _entityNoOptions = new TestContext<EntityNoOptions>(1, new[] { typeof(PositionComponent) }).CreateEntity();
            _entityAggressiveInlining = new TestContext<EntityAggressiveInlining>(1, new[] { typeof(PositionComponent) }).CreateEntity();
        }

        [Benchmark]
        public void NoOptions()
        {
            _entityNoOptions.AddPosition(1, 2);
            _entityNoOptions.HasPosition();
            var unused = _entityNoOptions.GetPosition();
            _entityNoOptions.RemovePosition();
        }

        [Benchmark]
        public void AggressiveInlining()
        {
            _entityAggressiveInlining.AddPosition(1, 2);
            _entityAggressiveInlining.HasPosition();
            var unused = _entityAggressiveInlining.GetPosition();
            _entityAggressiveInlining.RemovePosition();
        }
    }

    public static class AggressiveInliningPositionEntityExtension
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool HasPosition(this EntityAggressiveInlining entity)
        {
            return entity.HasComponent(0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static EntityAggressiveInlining AddPosition(this EntityAggressiveInlining entity, int x, int y)
        {
            var index = 0;
            var component = (PositionComponent)entity.CreateComponent(index, typeof(PositionComponent));
            component.X = x;
            component.Y = y;
            entity.AddComponent(index, component);
            return entity;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static EntityAggressiveInlining ReplacePosition(this EntityAggressiveInlining entity, int x, int y)
        {
            var index = 0;
            var component = (PositionComponent)entity.CreateComponent(index, typeof(PositionComponent));
            component.X = x;
            component.Y = y;
            entity.ReplaceComponent(index, component);
            return entity;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static EntityAggressiveInlining RemovePosition(this EntityAggressiveInlining entity)
        {
            entity.RemoveComponent(0);
            return entity;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static PositionComponent GetPosition(this EntityAggressiveInlining entity)
        {
            return (PositionComponent)entity.GetComponent(0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this PositionComponent component, out int x, out int y)
        {
            x = component.X;
            y = component.Y;
        }
    }

    public static class PositionEntityExtension
    {
        public static bool HasPosition(this EntityNoOptions entity)
        {
            return entity.HasComponent(0);
        }

        public static EntityNoOptions AddPosition(this EntityNoOptions entity, int x, int y)
        {
            var index = 0;
            var component = (PositionComponent)entity.CreateComponent(index, typeof(PositionComponent));
            component.X = x;
            component.Y = y;
            entity.AddComponent(index, component);
            return entity;
        }

        public static EntityNoOptions ReplacePosition(this EntityNoOptions entity, int x, int y)
        {
            var index = 0;
            var component = (PositionComponent)entity.CreateComponent(index, typeof(PositionComponent));
            component.X = x;
            component.Y = y;
            entity.ReplaceComponent(index, component);
            return entity;
        }

        public static EntityNoOptions RemovePosition(this EntityNoOptions entity)
        {
            entity.RemoveComponent(0);
            return entity;
        }

        public static PositionComponent GetPosition(this EntityNoOptions entity)
        {
            return (PositionComponent)entity.GetComponent(0);
        }

        public static void Deconstruct(this PositionComponent component, out int x, out int y)
        {
            x = component.X;
            y = component.Y;
        }
    }

    public sealed class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }

    public sealed class EntityNoOptions : Entitas.Entity { }

    public sealed class EntityAggressiveInlining : Entitas.Entity { }

    public sealed class TestContext<TEntity> : Context<TEntity> where TEntity : class, IEntity, new()
    {
        public TestContext(int totalComponents, Type[] componentTypes)
            : base(
                totalComponents,
                0,
                new ContextInfo(
                    "Entitas.Benchmarks",
                    componentTypes.Select(type => type.FullName).ToArray(),
                    componentTypes
                ),
                SafeAERC.Delegate,
                () => new TEntity()
            ) { }
    }
}
