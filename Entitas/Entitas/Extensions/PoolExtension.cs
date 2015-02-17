namespace Entitas {
    public static class PoolExtension {
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }
    }
}

