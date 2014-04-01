using System;

namespace Entitas {
    public class AllOfEntityMatcher : AbstractEntityMatcher {
        public AllOfEntityMatcher(Type[] types) : base(types) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasComponents(types);
        }
    }
}
