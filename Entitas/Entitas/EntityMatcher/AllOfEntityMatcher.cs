using System;
using System.Collections.Generic;

namespace Entitas {
    public class AllOfEntityMatcher : AbstractEntityMatcher {
        public AllOfEntityMatcher(IEnumerable<Type> types) : base(types) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasComponents(_types);
        }
    }
}
