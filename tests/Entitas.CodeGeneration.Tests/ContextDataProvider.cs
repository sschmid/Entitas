using Entitas.CodeGeneration.Plugins;
using FluentAssertions;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    public class ContextDataProvider
    {
        [Fact]
        public void CreatesDataForEachContextName()
        {
            var names = "Entitas.CodeGeneration.Plugins.Contexts = Input, GameState";
            var provider = new Entitas.CodeGeneration.Plugins.ContextDataProvider();
            provider.Configure(new TestPreferences(names));

            var data = (ContextData[])provider.GetData();
            data.Length.Should().Be(2);
            data[0].GetContextName().Should().Be("Input");
            data[1].GetContextName().Should().Be("GameState");
        }
    }
}
