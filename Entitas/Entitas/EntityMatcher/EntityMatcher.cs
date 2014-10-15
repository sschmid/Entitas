namespace Entitas {
    public static class EntityMatcher {
        public static AllOfEntityMatcher AllOf(int[] indices) {
            return new AllOfEntityMatcher(indices);
        }
    }
}

