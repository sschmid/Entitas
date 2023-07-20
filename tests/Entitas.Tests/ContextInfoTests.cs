using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class ContextInfoTests
    {
        [Fact]
        public void SetsFieldsWithConstructorValues()
        {
            var contextName = "My Context";
            var componentNames = new[] {"Health", "Position", "View"};
            var componentTypes = new[] {typeof(ComponentA), typeof(ComponentB), typeof(ComponentC)};

            var info = new ContextInfo(contextName, componentNames, componentTypes);

            info.Name.Should().Be(contextName);
            info.ComponentNames.Should().BeSameAs(componentNames);
            info.ComponentTypes.Should().BeSameAs(componentTypes);
        }
    }
}
