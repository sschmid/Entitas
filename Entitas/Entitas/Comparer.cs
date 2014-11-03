using System.Collections.Generic;

namespace Entitas {
    public class EntityEqualityComparer : IEqualityComparer<Entity>{

        public static readonly EntityEqualityComparer comparer = new EntityEqualityComparer();

        public bool Equals(Entity x, Entity y) {
            return x == y;
        }

        public int GetHashCode(Entity obj) {
            return obj.creationIndex;
        }
    }

    public class EntityComponentPairEqualityComparer : IEqualityComparer<EntityComponentPair>{

        public static readonly EntityComponentPairEqualityComparer comparer = new EntityComponentPairEqualityComparer();

        public bool Equals(EntityComponentPair x, EntityComponentPair y) {
            return x == y;
        }

        public int GetHashCode(EntityComponentPair obj) {
            return obj.entity.creationIndex;
        }
    }
}

