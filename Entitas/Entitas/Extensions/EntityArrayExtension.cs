using System.Collections.Generic;
using System;

namespace Entitas {
    public static class EntityArrayExtension {
        public static Entity[] With(this Entity[] entities, IEnumerable<Type> types) {
            var with = new List<Entity>();
            foreach (var e in entities)
                if (e.HasComponents(types))
                    with.Add(e);

            return with.ToArray();
        }

        public static Entity[] Without(this Entity[] entities, IEnumerable<Type> types) {
            var without = new List<Entity>();
            foreach (var e in entities)
                if (!e.HasAnyComponent(types))
                    without.Add(e);

            return without.ToArray();
        }
    }
}

