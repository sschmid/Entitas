namespace Entitas {
    public static class EntityRepositoryExtension {
        public static EntityCollection GetCollection(this EntityRepository repo, params int[] indices) {
            return repo.GetCollection(EntityMatcher.AllOf(indices));
        }
    }
}

