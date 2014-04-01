using System;

namespace Entitas {
    public static class EntityMatcher {
        public static IEntityMatcher AllOf(Type[] types) {
            return new AllOfEntityMatcher(types);
        }
    }
}

