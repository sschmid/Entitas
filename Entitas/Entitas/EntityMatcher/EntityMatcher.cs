using System;
using System.Collections.Generic;

namespace Entitas {
    public static class EntityMatcher {
        public static IEntityMatcher AllOf(IEnumerable<Type> types) {
            return new AllOfEntityMatcher(types);
        }

        public static IEntityMatcher AnyOf(IEnumerable<Type> types) {
            return new AnyOfEntityMatcher(types);
        }

        public static IEntityMatcher AllOfExcept(IEnumerable<Type> types, IEnumerable<Type> except) {
            return new AllOfExceptEntityMatcher(types, except);
        }
    }
}

