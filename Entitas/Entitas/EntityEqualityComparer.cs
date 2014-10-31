using System.Collections.Generic;

namespace Entitas {
    public class EntityEqualityComparer : IEqualityComparer<Entity>{
        public bool Equals(Entity x, Entity y) {
            return x == y;
        }

        public int GetHashCode(Entity obj) {
            return obj.creationIndex;
        }
    }
}

