﻿using System.Collections.Generic;

namespace Entitas {
    public static class CollectionExtension {

        /// Returns the only entity in the collection. It will throw an exception if the collection doesn't have exactly one entity.
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

