using System;
using System.Collections.Generic;

namespace Entitas {
    public static partial class Matcher {
        public static AllOfMatcher AllOf(params int[] indices) {
            return new AllOfMatcher(indices);
        }

        public static AllOfMatcher AllOf(params AllOfMatcher[] matchers) {
            var indices = new List<int>();
            foreach (var matcher in matchers) {
                indices.AddRange(matcher.indices);
            }

            return AllOf(indices.ToArray());
        }
    }

    public class MatcherException : Exception {
        public MatcherException(IMatcher matcher) :
        base("Matcher must have exactely one index, had " + matcher.indices.Length + ".\n" + matcher) {
        }
    }
}
