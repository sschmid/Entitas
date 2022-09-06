using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntitasResourcesTests
    {
        [Fact]
        public void GetsVersion()
        {
            EntitasResources.GetVersion().Should().NotBeNull();
        }
    }
}
