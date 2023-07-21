using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntitasExceptionTests
    {
        [Fact]
        public void CreatesExceptionWithHintSeparatedByNewLine()
        {
            new EntitasException("Message", "Hint").Message
                .Should().Be("Message\nHint");
        }

        [Fact]
        public void IgnoresHintWhenNull()
        {
            new EntitasException("Message", null).Message
                .Should().Be("Message");
        }
    }
}
