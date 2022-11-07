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
            AssertSameWord(Word + Entitas.EntitasStringExtension.CONTEXT_SUFFIX, (Word + Entitas.EntitasStringExtension.CONTEXT_SUFFIX).AddContextSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.ENTITY_SUFFIX, (Word + Entitas.EntitasStringExtension.ENTITY_SUFFIX).AddEntitySuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.COMPONENT_SUFFIX, (Word + Entitas.EntitasStringExtension.COMPONENT_SUFFIX).AddComponentSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.SYSTEM_SUFFIX, (Word + Entitas.EntitasStringExtension.SYSTEM_SUFFIX).AddSystemSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.MATCHER_SUFFIX, (Word + Entitas.EntitasStringExtension.MATCHER_SUFFIX).AddMatcherSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.LISTENER_SUFFIX, (Word + Entitas.EntitasStringExtension.LISTENER_SUFFIX).AddListenerSuffix());
        }

        [Fact]
        public void AddsSuffixToStringNotEndingWithSuffix()
        {
            AssertSameWord(Word + Entitas.EntitasStringExtension.CONTEXT_SUFFIX, Word.AddContextSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.ENTITY_SUFFIX, Word.AddEntitySuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.COMPONENT_SUFFIX, Word.AddComponentSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.SYSTEM_SUFFIX, Word.AddSystemSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.MATCHER_SUFFIX, Word.AddMatcherSuffix());
            AssertSameWord(Word + Entitas.EntitasStringExtension.LISTENER_SUFFIX, Word.AddListenerSuffix());
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
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.CONTEXT_SUFFIX).RemoveContextSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.ENTITY_SUFFIX).RemoveEntitySuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.COMPONENT_SUFFIX).RemoveComponentSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.SYSTEM_SUFFIX).RemoveSystemSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.MATCHER_SUFFIX).RemoveMatcherSuffix());
            AssertSameWord(Word, (Word + Entitas.EntitasStringExtension.LISTENER_SUFFIX).RemoveListenerSuffix());
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
            (Word + Entitas.EntitasStringExtension.CONTEXT_SUFFIX).HasContextSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.ENTITY_SUFFIX).HasEntitySuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.COMPONENT_SUFFIX).HasComponentSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.SYSTEM_SUFFIX).HasSystemSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.MATCHER_SUFFIX).HasMatcherSuffix().Should().BeTrue();
            (Word + Entitas.EntitasStringExtension.LISTENER_SUFFIX).HasListenerSuffix().Should().BeTrue();
        }

        void AssertSameWord(string word1, string word2) => word1.Should().Be(word2);
    }
}
