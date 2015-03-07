namespace Entitas {
    public class NoneOfMatcher : AbstractMatcher {
        public NoneOfMatcher(int[] indices) : base(indices) {
        }

        public override bool Matches(Entity entity) {
            for (int i = 0, indicesLength = indices.Length; i < indicesLength; i++) {
                if (entity.HasComponent(indices[i])) {
                    return false;
                }
            }

            return true;
        }
    }

    public static partial class Matcher {
        public static NoneOfMatcher NoneOf(params int[] indices) {
            return new NoneOfMatcher(indices);
        }
    }
}

