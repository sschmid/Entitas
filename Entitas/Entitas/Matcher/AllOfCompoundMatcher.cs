namespace Entitas {
    public class AllOfCompoundMatcher : AbstractCompoundMatcher {
        public AllOfCompoundMatcher(IMatcher[] matchers) : base(matchers) {
        }

        public override bool Matches(Entity entity) {
            foreach (var matcher in matchers) {
                if (!matcher.Matches(entity)) {
                    return false;
                }
            }

            return true;
        }
    }
}

