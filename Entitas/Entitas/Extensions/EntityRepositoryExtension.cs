namespace Entitas {
    public static class EntityRepositoryExtension {
        public static EntityCollection GetCollection(this EntityRepository repo, params int[] indices) {
            return repo.GetCollection(EntityMatcher.AllOf(indices));
        }

        public static Entity[] GetEntities(this EntityRepository repo, IEntityMatcher matcher) {
            return repo.GetCollection(matcher).GetEntities();
        }

        public static Entity[] GetEntities(this EntityRepository repo, params int[] indices) {
            return repo.GetCollection(indices).GetEntities();
        }

        public static EntityRepositoryObserver AddObserver(this EntityRepository repo, EntityCollectionEventType eventType, IEntityMatcher matcher) {
            return new EntityRepositoryObserver(repo, eventType, matcher);
        }

        public static EntityRepositoryObserver AddObserver(this EntityRepository repo, EntityCollectionEventType eventType, params int[] indices) {
            return new EntityRepositoryObserver(repo, eventType, EntityMatcher.AllOf(indices));
        }

        public static EntityWillBeRemovedEntityRepositoryObserver AddWillBeRemovedObserver(this EntityRepository repo, int index) {
            return new EntityWillBeRemovedEntityRepositoryObserver(repo, index);
        }
    }
}

