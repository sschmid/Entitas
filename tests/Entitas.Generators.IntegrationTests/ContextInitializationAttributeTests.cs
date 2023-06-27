using System.Linq;
using FluentAssertions;
using MyApp;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextInitializationAttributeTests
    {
        [Fact]
        public void ContextInitializationAttribute()
        {
            new MyApp.Main.ContextInitializationAttribute().Should().BeAssignableTo<System.Attribute>();
        }

        [Fact]
        public void StripsAttributes()
        {
            typeof(ContextInitialization)
                .GetMethod(nameof(ContextInitialization.Initialize))!
                .CustomAttributes
                .Where(data => data.AttributeType.Name.EndsWith("ContextInitializationAttribute"))
                .Should().HaveCount(0);
        }
    }
}
