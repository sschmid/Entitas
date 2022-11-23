using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntitasStringExtension
    {
        const string Word = "Word";

        [Fact]
        public void DoesNotAddSuffixToStringEndingWithSuffix()
        {
            AssertSameWord(Word + Entitas.EntitasStringExtension.ContextSuffix, (Word + Entitas.EntitasStringExtension.ContextSuffix).AddContextSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.EntitySuffix, (Word + Entitas.EntitasStringExtension.EntitySuffix).AddEntitySuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.ComponentSuffix, (Word + Entitas.EntitasStringExtension.ComponentSuffix).AddComponentSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.SystemSuffix, (Word + Entitas.EntitasStringExtension.SystemSuffix).AddSystemSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.MatcherSuffix, (Word + Entitas.EntitasStringExtension.MatcherSuffix).AddMatcherSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.ListenerSuffix, (Word + Entitas.EntitasStringExtension.ListenerSuffix).AddListenerSuffix());
        }

        [Fact]
        public void AddsSuffixToStringNotEndingWithSuffix()
        {
            AssertSameWord(Word + Entitas.EntitasStringExtension.ContextSuffix, Word.AddContextSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.EntitySuffix, Word.AddEntitySuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.ComponentSuffix, Word.AddComponentSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.SystemSuffix, Word.AddSystemSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.MatcherSuffix, Word.AddMatcherSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.ListenerSuffix, Word.AddListenerSuffix());
        }

        [Fact]
        public void DoesNotChangeStringWhenNotEndingWithSuffix()
        {
            AssertSameWord(Word, Word.RemoveContextSuffix());
            AssertSameWord(Word, Word.RemoveEntitySuffix());
            AssertSameWord(Word, Word.RemoveComponentSuffix());
            AssertSameWord(Word, Word.RemoveSystemSuffix());
            AssertSameWord(Word, Word.RemoveMatcherSuffix());
            AssertSameWord(Word, Word.RemoveListenerSuffix());
        }

        [Fact]
        public void RemovesSuffixWhenEndingWithSuffix()
        {
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.ContextSuffix).RemoveContextSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.EntitySuffix).RemoveEntitySuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.ComponentSuffix).RemoveComponentSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.SystemSuffix).RemoveSystemSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.MatcherSuffix).RemoveMatcherSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.ListenerSuffix).RemoveListenerSuffix());
        }

        [Fact]
        public void DoesNotHaveSuffix()
        {
            Word.HasContextSuffix().Should().BeFalse();
            Word.HasEntitySuffix().Should().BeFalse();
            Word.HasComponentSuffix().Should().BeFalse();
            Word.HasSystemSuffix().Should().BeFalse();
            Word.HasMatcherSuffix().Should().BeFalse();
            Word.HasListenerSuffix().Should().BeFalse();
        }

        [Fact]
        public void HasSuffix()
        {
            (Word + Entitas.EntitasStringExtension.ContextSuffix).HasContextSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.EntitySuffix).HasEntitySuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.ComponentSuffix).HasComponentSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.SystemSuffix).HasSystemSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.MatcherSuffix).HasMatcherSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.ListenerSuffix).HasListenerSuffix().Should().BeTrue();
        }

        void AssertSameWord(string word1, string word2) => word1.Should().Be(word2);
    }
}
