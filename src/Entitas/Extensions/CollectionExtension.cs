using System.Collections.Generic;
using System.Linq;

namespace Entitas
{
    public static class CollectionExtension
    {
        /// Returns the only entity in the collection.
        /// It will throw an exception if the collection doesn't have
        /// exactly one entity.
        public static IEntity SingleEntity(this ICollection<IEntity> collection)
        {
            if (collection.Count != 1)
                throw new SingleEntityException(collection.Count);

            return collection.First();
        }

        /// Returns the only entity in the collection.
        /// It will throw an exception if the collection doesn't have
        /// exactly one entity.
        public static TEntity SingleEntity<TEntity>(this ICollection<TEntity> collection) where TEntity : class, IEntity
        {
            if (collection.Count != 1)
                throw new SingleEntityException(collection.Count);

            return collection.First();
        }
    }

    public class SingleEntityException : EntitasException
    {
        public SingleEntityException(int count) :
            base($"Expected exactly one entity in collection but found {count}!",
                "Use collection.SingleEntity() only when you are sure that there is exactly one entity.") { }
    }
}
