using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class ContextDataProviderTests
    {
        [Fact]
        public void CreatesDataForEachContext()
        {
            var contexts = "Entitas.Plugins.Contexts = Input, GameState";
            var provider = new ContextDataProvider();
            provider.Configure(new TestPreferences(contexts));

            var data = (ContextData[])provider.GetData();
            data.Length.Should().Be(2);

            data[0].Name.Should().Be("Input");
            data[0].Type.Should().Be("InputContext");
            data[0].EntityType.Should().Be("InputEntity");
            data[0].MatcherType.Should().Be("InputMatcher");

            data[1].Name.Should().Be("GameState");
            data[1].Type.Should().Be("GameStateContext");
            data[1].EntityType.Should().Be("GameStateEntity");
            data[1].MatcherType.Should().Be("GameStateMatcher");
        }

        [Fact]
        public void RemovesContextSuffixFromContextName()
        {
            var contexts = "Entitas.Plugins.Contexts = InputContext, GameStateContext";
            var provider = new ContextDataProvider();
            provider.Configure(new TestPreferences(contexts));

            var data = (ContextData[])provider.GetData();
            data.Length.Should().Be(2);

            data[0].Name.Should().Be("Input");
            data[0].Type.Should().Be("InputContext");
            data[0].EntityType.Should().Be("InputEntity");
            data[0].MatcherType.Should().Be("InputMatcher");

            data[1].Name.Should().Be("GameState");
            data[1].Type.Should().Be("GameStateContext");
            data[1].EntityType.Should().Be("GameStateEntity");
            data[1].MatcherType.Should().Be("GameStateMatcher");
        }
    }
}
