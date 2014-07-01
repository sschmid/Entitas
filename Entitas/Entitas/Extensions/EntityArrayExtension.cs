using System;
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

        public static Entity SingleEntity(this List<Entity> list) {
            if (list.Count != 1)
                throw new Exception("Expected exactly one entity!");

            return list[0];
        }

        public static Entity Last(this List<Entity> list) {
            return list[list.Count - 1];
        }
    }
}

