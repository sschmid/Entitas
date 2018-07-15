using Entitas;
using NSpec;

class describe_EntitasStringExtension : nspec {

    const string WORD = "Word";

    void assertSameWord(string word1, string word2) {
        word1.should_be(word2);
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
                WORD.HasContextSuffix().should_be_false();
                WORD.HasEntitySuffix().should_be_false();
                WORD.HasComponentSuffix().should_be_false();
                WORD.HasSystemSuffix().should_be_false();
                WORD.HasMatcherSuffix().should_be_false();
                WORD.HasListenerSuffix().should_be_false();
            };

            it["has suffix"] = () => {
                (WORD + EntitasStringExtension.CONTEXT_SUFFIX).HasContextSuffix().should_be_true();
                (WORD + EntitasStringExtension.ENTITY_SUFFIX).HasEntitySuffix().should_be_true();
                (WORD + EntitasStringExtension.COMPONENT_SUFFIX).HasComponentSuffix().should_be_true();
                (WORD + EntitasStringExtension.SYSTEM_SUFFIX).HasSystemSuffix().should_be_true();
                (WORD + EntitasStringExtension.MATCHER_SUFFIX).HasMatcherSuffix().should_be_true();
                (WORD + EntitasStringExtension.LISTENER_SUFFIX).HasListenerSuffix().should_be_true();
            };
        };
    }
}
