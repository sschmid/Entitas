using System;
using System.Collections.Generic;

namespace Entitas {
    public static class EntityArrayExtension {
        public static List<Entity> With(this IEnumerable<Entity> entities, AllOfEntityMatcher matcher) {
            var with = new List<Entity>();
            foreach (var e in entities) {
                if (e.HasComponents(matcher.indices)) {
                    with.Add(e);
                }
            }

            return with;
        }

        public static List<Entity> Without(this IEnumerable<Entity> entities, AllOfEntityMatcher matcher) {
            var without = new List<Entity>();
            foreach (var e in entities) {
                if (!e.HasAnyComponent(matcher.indices)) {
                    without.Add(e);
                }
            }

            return without;
        }

        public static Entity SingleEntity(this List<Entity> list) {
            if (list.Count != 1) {
                throw new Exception("Expected exactly one entity!");
            }

            return list[0];
        }

        public static Entity Last(this IList<Entity> list) {
            return list[list.Count - 1];
        }
    }
}

