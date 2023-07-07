using FluentAssertions;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class MatcherTests
    {
        [Fact]
        public void GeneratesAllOfMatcher()
        {
            var matcher = MyApp.Main.Matcher.AllOf(1);
            matcher.Should().BeAssignableTo<IAllOfMatcher<MyApp.Main.Entity>>();
            matcher.Should().BeEquivalentTo(Entitas.Matcher<MyApp.Main.Entity>.AllOf(1));
        }

        [Fact]
        public void GeneratesAnyOfMatcher()
        {
            var matcher = MyApp.Main.Matcher.AnyOf(1);
            matcher.Should().BeAssignableTo<IAnyOfMatcher<MyApp.Main.Entity>>();
            matcher.Should().BeEquivalentTo(Entitas.Matcher<MyApp.Main.Entity>.AnyOf(1));
        }

        [Fact]
        public void GeneratesMatcherExtensions()
        {
            var matcher = MyApp.Main.Matcher
                .AllOf(1)
                .AnyOf(2)
                .NoneOf(3);

            matcher.Should().BeAssignableTo<INoneOfMatcher<MyApp.Main.Entity>>();

            var entitasMatcher = Entitas.Matcher<MyApp.Main.Entity>
                .AllOf(1)
                .AnyOf(2)
                .NoneOf(3);

            matcher.Should().BeEquivalentTo(entitasMatcher);
        }
    }
}
