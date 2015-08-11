using System.Collections.Generic;

namespace Entitas {
    public class AllOfMatcher : AbstractMatcher {
        public AllOfMatcher(int[] indices) : base(indices) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasComponents(indices);
        }
    }

    public partial class Matcher {
        public static AllOfMatcher AllOf(params int[] indices) {
            return new AllOfMatcher(indices);
        }

        public static AllOfMatcher AllOf(params AllOfMatcher[] matchers) {
            var indices = new List<int>();
            for (int i = 0, matchersLength = matchers.Length; i < matchersLength; i++) {
                indices.AddRange(matchers[i].indices);
            }

            return new AllOfMatcher(indices.ToArray());
        }
    }
}
