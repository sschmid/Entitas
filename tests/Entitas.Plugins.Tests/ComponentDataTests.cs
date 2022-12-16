using Entitas.Plugins.Attributes;
using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class ComponentDataTests
    {
        ComponentData _data;

        public ComponentDataTests()
        {
            _data = new ComponentData(
                type: "MyNamespace.TestComponent",
                memberData: new[] {new MemberData("string", "Value")},
                context: "Game",
                isUnique: true,
                flagPrefix: "Is",
                generates: true,
                generatesIndex: true,
                generatesObject: true,
                objectType: "string",
                eventData: new[] {new EventData(EventTarget.Self, EventType.Added, 42)}
            );
        }

        [Fact]
        public void SetsFields()
        {
            _data.Type.Should().Be("MyNamespace.TestComponent");
            _data.Name.Should().Be("Test");
            _data.ValidLowerFirstName.Should().Be("test");

            _data.MemberData.Should().HaveCount(1);
            _data.MemberData[0].Type.Should().Be("string");
            _data.MemberData[0].Name.Should().Be("Value");
            _data.Context.Should().Be("Game");
            _data.IsUnique.Should().BeTrue();
            _data.FlagPrefix.Should().Be("Is");
            _data.Generates.Should().BeTrue();
            _data.GeneratesIndex.Should().BeTrue();
            _data.GeneratesObject.Should().BeTrue();
            _data.ObjectType.Should().Be("string");
            _data.EventData.Should().HaveCount(1);
            _data.EventData[0].EventTarget.Should().Be(EventTarget.Self);
            _data.EventData[0].EventType.Should().Be(EventType.Added);
            _data.EventData[0].Order.Should().Be(42);
        }

        [Fact]
        public void ReplacesPlaceholders()
        {
            _data.ReplacePlaceholders("${Component.Type}").Should().Be("MyNamespace.TestComponent");
            _data.ReplacePlaceholders("${Component.Name}").Should().Be("Test");
            _data.ReplacePlaceholders("${Component.ValidLowerFirstName}").Should().Be("test");
            _data.ReplacePlaceholders("${Component.Context}").Should().Be("Game");
            _data.ReplacePlaceholders("${Component.IsUnique}").Should().Be("True");
            _data.ReplacePlaceholders("${Component.FlagPrefix}").Should().Be("Is");
            _data.ReplacePlaceholders("${Component.Generates}").Should().Be("True");
            _data.ReplacePlaceholders("${Component.GeneratesIndex}").Should().Be("True");
            _data.ReplacePlaceholders("${Component.GeneratesObject}").Should().Be("True");
            _data.ReplacePlaceholders("${Component.ObjectType}").Should().Be("string");
        }
    }
}
