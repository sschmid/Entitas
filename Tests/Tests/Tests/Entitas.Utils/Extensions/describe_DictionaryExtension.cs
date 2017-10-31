using NSpec;
using System.Collections.Generic;
using Entitas.Utils;

class describe_DictionaryExtension : nspec {

    void when_dictionary() {

        it["merges dictionary"] = () => {
            var d1 = new Dictionary<string, string> {
                { "k1", "v1"},
                { "k2", "v2"}
            };

            var d2 = new Dictionary<string, string> {
                { "k1", "v1"},
                { "k3", "v3"}
            };

            var d3 = new Dictionary<string, string> {
                { "k1", "v1"},
                { "k4", "v4"}
            };

            var merged = d1.Merge(d2, d3);

            merged.Count.should_be(4);
            merged.Keys.should_contain("k1");
            merged.Keys.should_contain("k2");
            merged.Keys.should_contain("k3");
            merged.Keys.should_contain("k4");
        };
    }
}
