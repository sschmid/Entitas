using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntitasStringExtension
    {
        [Fact]
        public void DoesNotChangeStringWhenNotEndingWithSuffix()
        {
            "Word".RemoveSuffix("Test").Should().Be("Word");
        }

        [Fact]
        public void RemovesSuffixWhenEndingWithSuffix()
        {
            "WordTest".RemoveSuffix("Test").Should().Be("Word");
        }
    }
}
