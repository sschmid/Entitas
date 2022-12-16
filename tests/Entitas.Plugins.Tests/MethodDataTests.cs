using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class MethodDataTests
    {
        [Fact]
        public void SetsFields()
        {
            var methodData = new MethodData("string", "Test", new[] {new MemberData("string", "value")});
            methodData.ReturnType.Should().Be("string");
            methodData.MethodName.Should().Be("Test");
            methodData.Parameters.Should().HaveCount(1);
            methodData.Parameters[0].Type.Should().Be("string");
            methodData.Parameters[0].Name.Should().Be("value");
        }
    }
}
