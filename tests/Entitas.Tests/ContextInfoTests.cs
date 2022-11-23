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

            info.Name.Should().Be(context);
            info.ComponentNames.Should().BeSameAs(componentNames);
            info.ComponentTypes.Should().BeSameAs(componentTypes);
        }
    }
}
