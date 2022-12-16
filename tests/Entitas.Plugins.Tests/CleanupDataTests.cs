using Entitas.Plugins.Attributes;
using FluentAssertions;
using Jenny;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class CleanupDataTests
    {
        readonly ComponentData _componentData = new ComponentData(new CodeGeneratorData());
        readonly CleanupData _data;

        public CleanupDataTests()
        {
            _data = new CleanupData(_componentData, CleanupMode.DestroyEntity);
        }

        [Fact]
        public void SetsFields()
        {
            _data.ComponentData.Should().BeSameAs(_componentData);
            _data.CleanupMode.Should().Be(CleanupMode.DestroyEntity);
        }

        [Fact]
        public void ReplacesPlaceholders()
        {
            _data.ReplacePlaceholders("${Cleanup.Mode}").Should().Be("DestroyEntity");
        }
    }
}
