using System;
using System.Collections.Generic;

namespace Entitas {
    public static partial class Matcher {
        public static AllOfEntityMatcher AllOf(params int[] indices) {
            return new AllOfEntityMatcher(indices);
        }

        public static AllOfEntityMatcher AllOf(params AllOfEntityMatcher[] matchers) {
            var indices = new List<int>();
            foreach (var matcher in matchers) {
                indices.AddRange(matcher.indices);
            }

            return AllOf(indices.ToArray());
        }
    }

    public class MatcherException : Exception {
        public MatcherException(IEntityMatcher matcher) :
        base("Matcher must have exactely one index, had " + matcher.indices.Length + ".\n" + matcher) {
        }
    }
}
