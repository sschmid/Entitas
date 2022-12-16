using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class MemberDataTests
    {
        [Fact]
        public void SetsFields()
        {
            var memberData = new MemberData("string", "value");
            memberData.Type.Should().Be("string");
            memberData.Name.Should().Be("value");
        }
    }
}
