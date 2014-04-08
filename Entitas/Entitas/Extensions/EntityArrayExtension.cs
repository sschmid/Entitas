using System.Collections.Generic;

namespace Entitas {
    public static class EntityArrayExtension {
        public static List<Entity> With(this Entity[] entities, int[] indices) {
            var with = new List<Entity>();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                var e = entities[i];
                if (e.HasComponents(indices))
                    with.Add(e);
            }

            return with;
        }

        public static List<Entity> Without(this Entity[] entities, int[] indices) {
            var without = new List<Entity>();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                var e = entities[i];
                if (!e.HasAnyComponent(indices))
                    without.Add(e);
            }

            return without;
        }

        public static List<Entity> With(this List<Entity> entities, int[] indices) {
            var with = new List<Entity>();
            for (int i = 0, entitiesLength = entities.Count; i < entitiesLength; i++) {
                var e = entities[i];
                if (e.HasComponents(indices))
                    with.Add(e);
            }

            return with;
        }

        public static List<Entity> Without(this List<Entity> entities, int[] indices) {
            var without = new List<Entity>();
            for (int i = 0, entitiesLength = entities.Count; i < entitiesLength; i++) {
                var e = entities[i];
                if (!e.HasAnyComponent(indices))
                    without.Add(e);
            }

            return without;
        }
    }
}

