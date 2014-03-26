using System;

namespace Entitas {
    public interface IEntityMatcher {
        bool Matches(Entity entity);

        bool HasType(Type type);
    }
}
