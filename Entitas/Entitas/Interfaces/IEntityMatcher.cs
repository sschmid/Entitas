using System;

namespace Entitas {
    public interface IEntityMatcher {
        Type[] types { get; }

        bool Matches(Entity entity);
    }
}
