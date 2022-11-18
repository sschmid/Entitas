using System;
using Jenny;
using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class EntityIndexGeneratorTests
    {
        [Fact]
        public void DoesNotGenerateWhenNoIndexesSpecified()
        {
            new EntityIndexGenerator()
                .Generate(Array.Empty<CodeGeneratorData>())
                .Should().HaveCount(0);
        }
    }
}
