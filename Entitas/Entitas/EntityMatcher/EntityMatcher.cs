namespace Entitas {
    public static class EntityMatcher {
        public static IEntityMatcher AllOf(int[] indices) {
            return new AllOfEntityMatcher(indices);
        }
    }
}

