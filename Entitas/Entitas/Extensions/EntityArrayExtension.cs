using System.Collections.Generic;

namespace Entitas {
    public static class EntityArrayExtension {
        public static List<Entity> With(this Entity[] entities, int[] indices) {
            var with = new List<Entity>();
            foreach (var e in entities)
                if (e.HasComponents(indices))
                    with.Add(e);

            return with;
        }

        public static List<Entity> Without(this Entity[] entities, int[] indices) {
            var without = new List<Entity>();
            foreach (var e in entities)
                if (!e.HasAnyComponent(indices))
                    without.Add(e);

            return without;
        }
    }
}

