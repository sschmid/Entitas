using System.Collections.Generic;
using System.Linq;

namespace Entitas.Utils {

    public static class DictionaryExtension {

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params Dictionary<TKey, TValue>[] dictionaries) {
            var merged = dictionary;
            foreach (var dict in dictionaries) {
                merged = merged.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            return merged;
        }
    }
}
