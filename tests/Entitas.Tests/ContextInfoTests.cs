using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class ContextInfoTests
    {
        [Fact]
        public void SetsFieldsWithConstructorValues()
        {
            var context = "My Context";
            var componentNames = new[] {"Health", "Position", "View"};
            var componentTypes = new[] {typeof(ComponentA), typeof(ComponentB), typeof(ComponentC)};

            var info = new ContextInfo(context, componentNames, componentTypes);

            info.name.Should().Be(context);
            info.componentNames.Should().BeSameAs(componentNames);
            info.componentTypes.Should().BeSameAs(componentTypes);
        }
    }
}
