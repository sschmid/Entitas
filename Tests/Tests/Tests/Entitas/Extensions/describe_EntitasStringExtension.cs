using Entitas;
using NSpec;
using Shouldly;

class describe_EntitasStringExtension : nspec {

    const string WORD = "Word";

    void assertSameWord(string word1, string word2) {
        word1.ShouldBe(word2);
    }

    void when_string() {

        context["when adding suffix"] = () => {

            it["doesn't add suffix to string ending with suffix"] = () => {
                assertSameWord(WORD + EntitasStringExtension.CONTEXT_SUFFIX, WORD.AddContextSuffix());
                assertSameWord(WORD + EntitasStringExtension.ENTITY_SUFFIX, WORD.AddEntitySuffix());
                assertSameWord(WORD + EntitasStringExtension.COMPONENT_SUFFIX, WORD.AddComponentSuffix());
                assertSameWord(WORD + EntitasStringExtension.SYSTEM_SUFFIX, WORD.AddSystemSuffix());
                assertSameWord(WORD + EntitasStringExtension.MATCHER_SUFFIX, WORD.AddMatcherSuffix());
                assertSameWord(WORD + EntitasStringExtension.LISTENER_SUFFIX, WORD.AddListenerSuffix());
            };
            it["adds suffix to string not ending with suffix"] = () => {
                assertSameWord(WORD + EntitasStringExtension.CONTEXT_SUFFIX, WORD.AddContextSuffix());
                assertSameWord(WORD + EntitasStringExtension.ENTITY_SUFFIX, WORD.AddEntitySuffix());
                assertSameWord(WORD + EntitasStringExtension.COMPONENT_SUFFIX, WORD.AddComponentSuffix());
                assertSameWord(WORD + EntitasStringExtension.SYSTEM_SUFFIX, WORD.AddSystemSuffix());
                assertSameWord(WORD + EntitasStringExtension.MATCHER_SUFFIX, WORD.AddMatcherSuffix());
                assertSameWord(WORD + EntitasStringExtension.LISTENER_SUFFIX, WORD.AddListenerSuffix());
            };
        };

        context["when removing suffix"] = () => {

            it["doesn't change string when not ending with suffix"] = () => {
                assertSameWord(WORD, WORD.RemoveContextSuffix());
                assertSameWord(WORD, WORD.RemoveEntitySuffix());
                assertSameWord(WORD, WORD.RemoveComponentSuffix());
                assertSameWord(WORD, WORD.RemoveSystemSuffix());
                assertSameWord(WORD, WORD.RemoveMatcherSuffix());
                assertSameWord(WORD, WORD.RemoveListenerSuffix());
            };

            it["removes suffix when ending with suffix"] = () => {
                assertSameWord(WORD, (WORD + EntitasStringExtension.CONTEXT_SUFFIX).RemoveContextSuffix());
                assertSameWord(WORD, (WORD + EntitasStringExtension.ENTITY_SUFFIX).RemoveEntitySuffix());
                assertSameWord(WORD, (WORD + EntitasStringExtension.COMPONENT_SUFFIX).RemoveComponentSuffix());
                assertSameWord(WORD, (WORD + EntitasStringExtension.SYSTEM_SUFFIX).RemoveSystemSuffix());
                assertSameWord(WORD, (WORD + EntitasStringExtension.MATCHER_SUFFIX).RemoveMatcherSuffix());
                assertSameWord(WORD, (WORD + EntitasStringExtension.LISTENER_SUFFIX).RemoveListenerSuffix());
            };
        };

        context["checking for suffix"] = () => {

            it["doesn't have suffix"] = () => {
                WORD.HasContextSuffix().ShouldBeFalse();
                WORD.HasEntitySuffix().ShouldBeFalse();
                WORD.HasComponentSuffix().ShouldBeFalse();
                WORD.HasSystemSuffix().ShouldBeFalse();
                WORD.HasMatcherSuffix().ShouldBeFalse();
                WORD.HasListenerSuffix().ShouldBeFalse();
            };

            it["has suffix"] = () => {
                (WORD + EntitasStringExtension.CONTEXT_SUFFIX).HasContextSuffix().ShouldBeTrue();
                (WORD + EntitasStringExtension.ENTITY_SUFFIX).HasEntitySuffix().ShouldBeTrue();
                (WORD + EntitasStringExtension.COMPONENT_SUFFIX).HasComponentSuffix().ShouldBeTrue();
                (WORD + EntitasStringExtension.SYSTEM_SUFFIX).HasSystemSuffix().ShouldBeTrue();
                (WORD + EntitasStringExtension.MATCHER_SUFFIX).HasMatcherSuffix().ShouldBeTrue();
                (WORD + EntitasStringExtension.LISTENER_SUFFIX).HasListenerSuffix().ShouldBeTrue();
            };
        };
    }
}
