using System;
using System.Collections.Generic;

namespace Entitas {
    public static class EntitySetExtension {
        public static Entity SingleEntity(this ICollection<Entity> collection) {
            if (collection.Count != 1) {
                throw new Exception("Expected exactly one entity but found " + collection.Count);
            }

            return collection.First();
        }

        public static Entity First(this ICollection<Entity> collection) {
            return System.Linq.Enumerable.First(collection);
        }
    }
}

