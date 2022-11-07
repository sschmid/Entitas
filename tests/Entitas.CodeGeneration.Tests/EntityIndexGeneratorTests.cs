using System;
using Jenny;
using Entitas.CodeGeneration.Plugins;
using FluentAssertions;
using Xunit;

namespace Entitas.CodeGeneration.Tests
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
