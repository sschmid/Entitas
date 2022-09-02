using System.Collections.Generic;

namespace Entitas {

    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : class, IEntity {

        public static readonly IEqualityComparer<TEntity> comparer = new EntityEqualityComparer<TEntity>();

        public bool Equals(TEntity x, TEntity y) {
            return x == y;
        }

        public int GetHashCode(TEntity obj) {
            return obj.creationIndex;
        }
    }
}
