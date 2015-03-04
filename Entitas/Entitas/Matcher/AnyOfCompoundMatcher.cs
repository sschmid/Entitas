namespace Entitas {
    public class AnyOfCompoundMatcher : AbstractCompoundMatcher {
        public AnyOfCompoundMatcher(IMatcher[] matchers) : base(matchers) {
        }

        public override bool Matches(Entity entity) {
            foreach (var matcher in matchers) {
                if (matcher.Matches(entity)) {
                    return true;
                }
            }

            return false;
        }
    }
}

