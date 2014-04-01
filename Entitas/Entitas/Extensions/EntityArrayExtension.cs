using System.Collections.Generic;
using System;

namespace Entitas {
    public static class EntityArrayExtension {
        public static List<Entity> With(this Entity[] entities, Type[] types) {
            var with = new List<Entity>();
            foreach (var e in entities)
                if (e.HasComponents(types))
                    with.Add(e);

            return with;
        }

        public static List<Entity> Without(this Entity[] entities, Type[] types) {
            var without = new List<Entity>();
            foreach (var e in entities)
                if (!e.HasAnyComponent(types))
                    without.Add(e);

            return without;
        }
    }
}

