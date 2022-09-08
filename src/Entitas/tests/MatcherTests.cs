using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class MatcherTests
    {
        readonly TestEntity _eA;
        readonly TestEntity _eB;
        readonly TestEntity _eC;
        readonly TestEntity _eAB;
        readonly TestEntity _eABC;

        public MatcherTests()
        {
            _eA = CreateEntity();
            _eA.AddComponentA();

            _eB = CreateEntity();
            _eB.AddComponentB();

            _eC = CreateEntity();
            _eC.AddComponentC();

            _eAB = CreateEntity();
            _eAB.AddComponentA();
            _eAB.AddComponentB();

            _eABC = CreateEntity();
            _eABC.AddComponentA();
            _eABC.AddComponentB();
            _eABC.AddComponentC();
        }

        [Fact]
        public void AllOfHasAllIndexes()
        {
            var matcher = CreateAllOfAB();
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.allOfIndices, CID.ComponentA, CID.ComponentB);
        }

        [Fact]
        public void AnyOfHasAllIndexes()
        {
            var matcher = CreateAnyOfAB();
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.anyOfIndices, CID.ComponentA, CID.ComponentB);
        }

        [Fact]
        public void AllOfNoneOfHasAllIndexes()
        {
            var matcher = CreateAllOfABNoneOfCD();
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC, CID.ComponentD);
            AssertIndexesEqual(matcher.allOfIndices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.noneOfIndices, CID.ComponentC, CID.ComponentD);
        }

        [Fact]
        public void AnyOfNoneOfHasAllIndexes()
        {
            var matcher = CreateAnyOfABNoneOfCD();
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC, CID.ComponentD);
            AssertIndexesEqual(matcher.anyOfIndices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.noneOfIndices, CID.ComponentC, CID.ComponentD);
        }

        [Fact]
        public void AllOfAnyOfHasAllIndexes()
        {
            var matcher = CreateAllOfABAnyOfCD();
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC, CID.ComponentD);
            AssertIndexesEqual(matcher.allOfIndices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.anyOfIndices, CID.ComponentC, CID.ComponentD);
        }

        [Fact]
        public void AllOfHasAllIndexesWithoutDuplicates()
        {
            var matcher = Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentA, CID.ComponentB, CID.ComponentB);
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.allOfIndices, CID.ComponentA, CID.ComponentB);
        }

        [Fact]
        public void AnyOfHasAllIndexesWithoutDuplicates()
        {
            var matcher = Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentA, CID.ComponentB, CID.ComponentB);
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.anyOfIndices, CID.ComponentA, CID.ComponentB);
        }

        [Fact]
        public void AllOfNoneOfHasAllIndexesWithoutDuplicates()
        {
            var matcher = Matcher<TestEntity>
                .AllOf(CID.ComponentA, CID.ComponentA, CID.ComponentB)
                .NoneOf(CID.ComponentB, CID.ComponentC, CID.ComponentC);
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            AssertIndexesEqual(matcher.allOfIndices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.noneOfIndices, CID.ComponentB, CID.ComponentC);
        }

        [Fact]
        public void AnyOfNoneOfHasAllIndexesWithoutDuplicates()
        {
            var matcher = Matcher<TestEntity>
                .AnyOf(CID.ComponentA, CID.ComponentA, CID.ComponentB)
                .NoneOf(CID.ComponentB, CID.ComponentC, CID.ComponentC);
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            AssertIndexesEqual(matcher.anyOfIndices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.noneOfIndices, CID.ComponentB, CID.ComponentC);
        }

        [Fact]
        public void AllOfAnyOfHasAllIndexesWithoutDuplicates()
        {
            var matcher = Matcher<TestEntity>
                .AllOf(CID.ComponentA, CID.ComponentA, CID.ComponentB)
                .AnyOf(CID.ComponentB, CID.ComponentC, CID.ComponentC);
            AssertIndexesEqual(matcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            AssertIndexesEqual(matcher.allOfIndices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(matcher.anyOfIndices, CID.ComponentB, CID.ComponentC);
        }

        [Fact]
        public void AllOfCachesIndexes()
        {
            var matcher = CreateAllOfAB();
            matcher.indices.Should().BeSameAs(matcher.indices);
        }

        [Fact]
        public void AnyOfCachesIndexes()
        {
            var matcher = CreateAnyOfAB();
            matcher.indices.Should().BeSameAs(matcher.indices);
        }

        [Fact]
        public void AllOfNoneOfCachesIndexes()
        {
            var matcher = CreateAllOfABNoneOfCD();
            matcher.indices.Should().BeSameAs(matcher.indices);
        }

        [Fact]
        public void AnyOfNoneOfCachesIndexes()
        {
            var matcher = CreateAnyOfABNoneOfCD();
            matcher.indices.Should().BeSameAs(matcher.indices);
        }

        [Fact]
        public void AllOfAnyOfCachesIndexes()
        {
            var matcher = CreateAllOfABAnyOfCD();
            matcher.indices.Should().BeSameAs(matcher.indices);
        }

        [Fact]
        public void AllOfDoesNotMatch()
        {
            CreateAllOfAB().Matches(_eA).Should().BeFalse();
        }

        [Fact]
        public void AnyOfDoesNotMatch()
        {
            CreateAnyOfAB().Matches(_eC).Should().BeFalse();
        }

        [Fact]
        public void AllOfNoneOfDoesNotMatch()
        {
            CreateAllOfABNoneOfCD().Matches(_eABC).Should().BeFalse();
        }

        [Fact]
        public void AnyOfNoneOfDoesNotMatch()
        {
            CreateAnyOfABNoneOfCD().Matches(_eABC).Should().BeFalse();
        }

        [Fact]
        public void AllOfAnyOfDoesNotMatch()
        {
            CreateAllOfABAnyOfCD().Matches(_eAB).Should().BeFalse();
        }

        [Fact]
        public void AllOfMatches()
        {
            var matcher = CreateAllOfAB();
            matcher.Matches(_eAB).Should().BeTrue();
            matcher.Matches(_eABC).Should().BeTrue();
        }

        [Fact]
        public void AnyOfMatches()
        {
            var matcher = CreateAnyOfAB();
            matcher.Matches(_eA).Should().BeTrue();
            matcher.Matches(_eB).Should().BeTrue();
            matcher.Matches(_eABC).Should().BeTrue();
        }

        [Fact]
        public void AllOfNoneOfMatches()
        {
            CreateAllOfABNoneOfCD().Matches(_eAB).Should().BeTrue();
        }

        [Fact]
        public void AnyOfNoneOfMatches()
        {
            var matcher = CreateAnyOfABNoneOfCD();
            matcher.Matches(_eA).Should().BeTrue();
            matcher.Matches(_eB).Should().BeTrue();
        }

        [Fact]
        public void AllOfAnyOfMatches()
        {
            CreateAllOfABAnyOfCD().Matches(_eABC).Should().BeTrue();
        }

        [Fact]
        public void AllOfMergesMatchersToNewMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AllOf(CID.ComponentC);
            var mergedMatcher = Matcher<TestEntity>.AllOf(m1, m2, m3);
            AssertIndexesEqual(mergedMatcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            AssertIndexesEqual(mergedMatcher.allOfIndices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
        }

        [Fact]
        public void AnyOfMergesMatchersToNewMatcher()
        {
            var m1 = Matcher<TestEntity>.AnyOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AnyOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AnyOf(CID.ComponentC);
            var mergedMatcher = Matcher<TestEntity>.AnyOf(m1, m2, m3);
            AssertIndexesEqual(mergedMatcher.indices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
            AssertIndexesEqual(mergedMatcher.anyOfIndices, CID.ComponentA, CID.ComponentB, CID.ComponentC);
        }

        [Fact]
        public void AllOfMergesMatchersToNewMatcherWithoutDuplicates()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m3 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var mergedMatcher = Matcher<TestEntity>.AllOf(m1, m2, m3);
            AssertIndexesEqual(mergedMatcher.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(mergedMatcher.allOfIndices, CID.ComponentA, CID.ComponentB);
        }

        [Fact]
        public void AnyOfMergesMatchersToNewMatcherWithoutDuplicates()
        {
            var m1 = Matcher<TestEntity>.AnyOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AnyOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AnyOf(CID.ComponentB);
            var mergedMatcher = Matcher<TestEntity>.AnyOf(m1, m2, m3);
            AssertIndexesEqual(mergedMatcher.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(mergedMatcher.anyOfIndices, CID.ComponentA, CID.ComponentB);
        }

        [Fact]
        public void AllOfThrowsWhenMergingMatcherWithMoreThanOneIndex()
        {
            var matcher = Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB);
            FluentActions.Invoking(() => Matcher<TestEntity>.AllOf(matcher))
                .Should().Throw<MatcherException>();
        }

        [Fact]
        public void AnyOfThrowsWhenMergingMatcherWithMoreThanOneIndex()
        {
            var matcher = Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB);
            FluentActions.Invoking(() => Matcher<TestEntity>.AnyOf(matcher))
                .Should().Throw<MatcherException>();
        }

        [Fact]
        public void AllOfCanToString()
        {
            CreateAllOfAB().ToString().Should().Be("AllOf(1, 2)");
        }

        [Fact]
        public void AnyOfCanToString()
        {
            CreateAnyOfAB().ToString().Should().Be("AnyOf(1, 2)");
        }

        [Fact]
        public void AllOfNoneOfCanToString()
        {
            CreateAllOfABNoneOfCD().ToString().Should().Be("AllOf(1, 2).NoneOf(3, 4)");
        }

        [Fact]
        public void AnyOfNoneOfCanToString()
        {
            CreateAnyOfABNoneOfCD().ToString().Should().Be("AnyOf(1, 2).NoneOf(3, 4)");
        }

        [Fact]
        public void AllOfAnyOfCanToString()
        {
            CreateAllOfABAnyOfCD().ToString().Should().Be("AllOf(1, 2).AnyOf(3, 4)");
        }

        [Fact]
        public void ToStringUsesComponentNamesWhenSet()
        {
            var matcher = (Matcher<TestEntity>)CreateAllOfAB();
            matcher.componentNames = new[] {"one", "two", "three"};
            matcher.ToString().Should().Be("AllOf(two, three)");
        }

        [Fact]
        public void ToStringUsesComponentNamesWhenMergedMatcher()
        {
            var m1 = (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentC);
            m2.componentNames = new[] {"m_0", "m_1", "m_2", "m_3"};
            var mergedMatcher = Matcher<TestEntity>.AllOf(m1, m2, m3);
            mergedMatcher.ToString().Should().Be("AllOf(m_1, m_2, m_3)");
        }

        [Fact]
        public void AllOfNoneOfToStringUsesComponentNamesWhenComponentNamesSet()
        {
            var matcher = (Matcher<TestEntity>)CreateAllOfABNoneOfCD();
            matcher.componentNames = new[] {"one", "two", "three", "four", "five"};
            matcher.ToString().Should().Be("AllOf(two, three).NoneOf(four, five)");
        }

        [Fact]
        public void NoneOfMutatesAllOfMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = m1.NoneOf(CID.ComponentB);
            m1.Should().BeSameAs(m2);
            AssertIndexesEqual(m1.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(m1.allOfIndices, CID.ComponentA);
            AssertIndexesEqual(m1.noneOfIndices, CID.ComponentB);
        }

        [Fact]
        public void NoneOfMutatesAllOfMergedMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AllOf(m1);
            var m4 = m3.NoneOf(m2);
            m3.Should().BeSameAs(m4);
            AssertIndexesEqual(m3.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(m3.allOfIndices, CID.ComponentA);
            AssertIndexesEqual(m3.noneOfIndices, CID.ComponentB);
        }

        [Fact]
        public void NoneOfMutatesAnyOfMatcher()
        {
            var m1 = Matcher<TestEntity>.AnyOf(CID.ComponentA);
            var m2 = m1.NoneOf(CID.ComponentB);
            m1.Should().BeSameAs(m2);
            AssertIndexesEqual(m1.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(m1.anyOfIndices, CID.ComponentA);
            AssertIndexesEqual(m1.noneOfIndices, CID.ComponentB);
        }

        [Fact]
        public void NoneOfMutatesAnyOfMergedMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AnyOf(m1);
            var m4 = m3.NoneOf(m2);
            m3.Should().BeSameAs(m4);
            AssertIndexesEqual(m3.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(m3.anyOfIndices, CID.ComponentA);
            AssertIndexesEqual(m3.noneOfIndices, CID.ComponentB);
        }

        [Fact]
        public void AnyOfMutatesAllOfMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = m1.AnyOf(CID.ComponentB);
            m1.Should().BeSameAs(m2);
            AssertIndexesEqual(m1.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(m1.allOfIndices, CID.ComponentA);
            AssertIndexesEqual(m1.anyOfIndices, CID.ComponentB);
        }

        [Fact]
        public void AnyOfMutatesAllOfMergedMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AllOf(m1);
            var m4 = m3.AnyOf(m2);
            m3.Should().BeSameAs(m4);
            AssertIndexesEqual(m3.indices, CID.ComponentA, CID.ComponentB);
            AssertIndexesEqual(m3.allOfIndices, CID.ComponentA);
            AssertIndexesEqual(m3.anyOfIndices, CID.ComponentB);
        }

        [Fact]
        public void UpdatesCacheWhenCallingAnyOf()
        {
            var matcher = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var cache = matcher.indices;
            matcher.AnyOf(CID.ComponentB);
            matcher.indices.Should().NotBeSameAs(cache);
        }

        [Fact]
        public void UpdatesCacheWhenCallingNoneOf()
        {
            var matcher = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var cache = matcher.indices;
            matcher.NoneOf(CID.ComponentB);
            matcher.indices.Should().NotBeSameAs(cache);
        }

        [Fact]
        public void UpdatesHashWhenChangedWithAnyOf()
        {
            var matcher = AllOfAB();
            var hash = matcher.GetHashCode();
            matcher.AnyOf(42).GetHashCode().Should().NotBe(hash);
        }

        [Fact]
        public void UpdatesHashWhenChangedWithNoneOf()
        {
            var matcher = AllOfAB();
            var hash = matcher.GetHashCode();
            matcher.NoneOf(42).GetHashCode().Should().NotBe(hash);
        }

        [Fact]
        public void EqualsEqualAllOfMatcher()
        {
            var m1 = AllOfAB();
            var m2 = AllOfAB();
            m1.Should().NotBeSameAs(m2);
            m1.Equals(m2).Should().BeTrue();
            m1.GetHashCode().Should().Be(m2.GetHashCode());
        }

        [Fact]
        public void EqualsEqualAllOfMatcherIndependentOfTheOrderOfIndices()
        {
            var m1 = AllOfAB();
            var m2 = AllOfBA();
            m1.Equals(m2).Should().BeTrue();
            m1.GetHashCode().Should().Be(m2.GetHashCode());
        }

        [Fact]
        public void EqualsMergedMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = AllOfBA();

            var mergedMatcher = Matcher<TestEntity>.AllOf(m1, m2);
            mergedMatcher.Equals(m3).Should().BeTrue();
            mergedMatcher.GetHashCode().Should().Be(m3.GetHashCode());
        }

        [Fact]
        public void DoesNotEqualDifferentAllOfMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = AllOfAB();
            m1.Equals(m2).Should().BeFalse();
            m1.GetHashCode().Should().NotBe(m2.GetHashCode());
        }

        [Fact]
        public void AllOfDoesNotEqualAnyOfWithSameIndexes()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AnyOf(CID.ComponentA);
            m1.Equals(m2).Should().BeFalse();
            m1.GetHashCode().Should().NotBe(m2.GetHashCode());
        }

        [Fact]
        public void DoesNotEqualDifferentTypeMatchersWithSameIndexes()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AllOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AllOf(m1, m2);
            var m4 = Matcher<TestEntity>.AnyOf(m1, m2);
            m3.Equals(m4).Should().BeFalse();
            m3.GetHashCode().Should().NotBe(m4.GetHashCode());
        }

        [Fact]
        public void EqualsCompoundMatcher()
        {
            var m1 = Matcher<TestEntity>.AllOf(CID.ComponentA);
            var m2 = Matcher<TestEntity>.AnyOf(CID.ComponentB);
            var m3 = Matcher<TestEntity>.AnyOf(CID.ComponentC);
            var m4 = Matcher<TestEntity>.AnyOf(CID.ComponentD);

            var mX = Matcher<TestEntity>.AllOf(m1, m2).AnyOf(m3, m4);
            var mY = Matcher<TestEntity>.AllOf(m1, m2).AnyOf(m3, m4);

            mX.Equals(mY).Should().BeTrue();
            mX.GetHashCode().Should().Be(mY.GetHashCode());
        }

        static TestEntity CreateEntity()
        {
            var entity = new TestEntity();
            entity.Initialize(0, CID.TotalComponents, new Stack<IComponent>[CID.TotalComponents]);
            return entity;
        }

        static IAllOfMatcher<TestEntity> CreateAllOfAB() => Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB);
        static IAnyOfMatcher<TestEntity> CreateAnyOfAB() => Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB);

        static ICompoundMatcher<TestEntity> CreateAllOfABNoneOfCD() => Matcher<TestEntity>
            .AllOf(CID.ComponentA, CID.ComponentB)
            .NoneOf(CID.ComponentC, CID.ComponentD);

        static ICompoundMatcher<TestEntity> CreateAnyOfABNoneOfCD() => Matcher<TestEntity>
            .AnyOf(CID.ComponentA, CID.ComponentB)
            .NoneOf(CID.ComponentC, CID.ComponentD);

        static ICompoundMatcher<TestEntity> CreateAllOfABAnyOfCD() => Matcher<TestEntity>
            .AllOf(CID.ComponentA, CID.ComponentB)
            .AnyOf(CID.ComponentC, CID.ComponentD);

        static void AssertIndexesEqual(int[] indices, params int[] expectedIndices)
        {
            indices.Length.Should().Be(expectedIndices.Length);
            for (var i = 0; i < expectedIndices.Length; i++)
                indices[i].Should().Be(expectedIndices[i]);
        }

        static IAllOfMatcher<TestEntity> AllOfAB() => Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB);
        static IAllOfMatcher<TestEntity> AllOfBA() => Matcher<TestEntity>.AllOf(CID.ComponentB, CID.ComponentA);
    }
}
