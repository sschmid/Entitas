using System.Collections.Generic;

namespace Entitas
{
    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : class, IEntity
    {
        public static readonly IEqualityComparer<TEntity> Comparer = new EntityEqualityComparer<TEntity>();

        public bool Equals(TEntity x, TEntity y) => x == y;

        public int GetHashCode(TEntity obj) => obj.Id;
    }
}
