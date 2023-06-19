using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Entitas.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class ComponentIndexBenchmarks
    {
        [Benchmark]
        public int[] IntArray()
        {
            var componentIndexes = new[]
            {
                new ComponentIndex(0),
                new ComponentIndex(1),
                new ComponentIndex(2),
                new ComponentIndex(3),
                new ComponentIndex(4),
                new ComponentIndex(5)
            };

            return AllOf(componentIndexes);

            int[] AllOf(ComponentIndex[] indexes)
            {
                var ints = new int[indexes.Length];
                for (var i = 0; i < indexes.Length; i++)
                    ints[i] = indexes[i];

                return ints;
            }
        }

        [Benchmark]
        public int[] Span()
        {
            Span<ComponentIndex> indexes = stackalloc ComponentIndex[]
            {
                new ComponentIndex(0),
                new ComponentIndex(1),
                new ComponentIndex(2),
                new ComponentIndex(3),
                new ComponentIndex(4),
                new ComponentIndex(5)
            };

            return AllOfSpan(indexes);

            int[] AllOfSpan(Span<ComponentIndex> indexes)
            {
                var ints = new int[indexes.Length];
                for (var i = 0; i < indexes.Length; i++)
                    ints[i] = indexes[i];

                return ints;
            }
        }

        [Benchmark]
        public int[] RefSpan()
        {
            Span<ComponentIndex> componentIndexes = stackalloc ComponentIndex[]
            {
                new ComponentIndex(0),
                new ComponentIndex(1),
                new ComponentIndex(2),
                new ComponentIndex(3),
                new ComponentIndex(4),
                new ComponentIndex(5)
            };

            return AllOfRefSpan(ref componentIndexes);

            int[] AllOfRefSpan(ref Span<ComponentIndex> indexes)
            {
                var ints = new int[indexes.Length];
                for (var i = 0; i < indexes.Length; i++)
                    ints[i] = indexes[i];

                return ints;
            }
        }
    }

    public readonly struct ComponentIndex : System.IEquatable<ComponentIndex>
    {
        public static implicit operator int(ComponentIndex index) => index.Value;
        public static implicit operator ComponentIndex(int index) => new ComponentIndex(index);

        public readonly int Value;

        public ComponentIndex(int value)
        {
            Value = value;
        }

        public bool Equals(ComponentIndex other) => Value == other.Value;
#nullable enable
        public override bool Equals(object? obj) => obj is ComponentIndex other && Equals(other);
#nullable disable
        public override int GetHashCode() => Value;
    }
}
