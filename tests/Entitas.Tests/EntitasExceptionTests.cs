using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntitasExceptionTests
    {
        [Fact]
        public void CreatesExceptionWithHintSeparatedByNewLine()
        {
            const string msg = "Message";
            const string hint = "Hint";
            new EntitasException(msg, hint).Message
                .Should().Be($"{msg}\n{hint}");
        }

        [Fact]
        public void IgnoresHintWhenNull()
        {
            const string msg = "Message";
            var ex = new EntitasException(msg, null);
            ex.Message.Should().Be(msg);
        }
    }
}
