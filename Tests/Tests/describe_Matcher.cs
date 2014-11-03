using NSpec;
using Entitas;

class describe_Matcher : nspec {

    void when_merging_matchers() {
        it["merges matchers"] = () => {
            var m1 = EntityMatcher.AllOf(new [] { 0 });
            var m2 = EntityMatcher.AllOf(new [] { 1 });
            var merged = Matcher.AllOf(m1, m2);

            merged.indices.Length.should_be(2);
            merged.indices.should_contain(0);
            merged.indices.should_contain(1);
        };

        it["merged matcher doesn't contain duplicate indices"] = () => {
            var m1 = EntityMatcher.AllOf(new [] { 0 });
            var m2 = EntityMatcher.AllOf(new [] { 1 });
            var m3 = EntityMatcher.AllOf(new [] { 1 });
            var merged = Matcher.AllOf(m1, m2, m3);

            merged.indices.Length.should_be(2);
            merged.indices.should_contain(0);
            merged.indices.should_contain(1);
        };
    }
}

