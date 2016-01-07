using System;
using System.Collections.Generic;
using Entitas;

namespace Entitas {
    public static class CollectionExtension {
        public static Entity SingleEntity(this ICollection<Entity> collection) {
            if (collection.Count != 1) {
                throw new SingleEntityException(collection.Count);
            }

            return System.Linq.Enumerable.First(collection);
        }
    }

    public class SingleEntityException : EntitasException {
        public SingleEntityException(int count) :
            base("Expected exactly one entity in collection but found " + count + "!",
                "Use collection.SingleEntity() only when you are sure that there is exactly one entity.") {
        }
    }
}

