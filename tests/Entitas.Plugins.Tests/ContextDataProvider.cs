using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class ContextDataProvider
    {
        [Fact]
        public void CreatesDataForEachContext()
        {
            var contexts = "Entitas.Plugins.Contexts = Input, GameState";
            var provider = new Plugins.ContextDataProvider();
            provider.Configure(new TestPreferences(contexts));

            var data = (ContextData[])provider.GetData();
            data.Length.Should().Be(2);
            data[0].Name.Should().Be("Input");
            data[0].Type.Should().Be("InputContext");
            data[1].Name.Should().Be("GameState");
            data[1].Type.Should().Be("GameStateContext");
        }

        [Fact]
        public void RemovesContextSuffixFromContextName()
        {
            var contexts = "Entitas.Plugins.Contexts = InputContext, GameStateContext";
            var provider = new Plugins.ContextDataProvider();
            provider.Configure(new TestPreferences(contexts));

            var data = (ContextData[])provider.GetData();
            data.Length.Should().Be(2);
            data[0].Name.Should().Be("Input");
            data[0].Type.Should().Be("InputContext");
            data[1].Name.Should().Be("GameState");
            data[1].Type.Should().Be("GameStateContext");
        }
    }
}
