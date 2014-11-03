using System.Collections.Generic;

namespace Entitas {
    public static partial class Matcher {
        public static AllOfEntityMatcher AllOf(params AllOfEntityMatcher[] matchers) {
            var indices = new List<int>();
            foreach (var matcher in matchers) {
                indices.AddRange(matcher.indices);
            }

            return EntityMatcher.AllOf(indices.ToArray());
        }
    }
}
