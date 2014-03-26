using System;
using System.Collections.Generic;

namespace Entitas {
    public class AnyOfEntityMatcher : AbstractEntityMatcher {
        public AnyOfEntityMatcher(IEnumerable<Type> types) : base(types) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasAnyComponent(_types);
        }
    }
}
