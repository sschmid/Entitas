namespace Entitas {
    public static class EntityRepositoryExtension {
        public static EntityCollection GetCollection<T>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] { typeof(T) }));
        }

        public static EntityCollection GetCollection<T1, T2>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3, T4>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3),
                typeof(T4)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3, T4, T5>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3, T4, T5, T6>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3, T4, T5, T6, T7>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3, T4, T5, T6, T7, T8>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8)
            }));
        }

        public static EntityCollection GetCollection<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this EntityRepository repo) {
            return repo.GetCollection(EntityMatcher.AllOf(new [] {
                typeof(T1), 
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9)
            }));
        }
    }
}

