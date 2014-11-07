namespace Entitas {
    public static class EntityRepositoryExtension {
        public static Entity[] GetEntities(this EntityRepository repo, IEntityMatcher matcher) {
            return repo.GetCollection(matcher).GetEntities();
        }

        public static EntityRepositoryObserver CreateObserver(this EntityRepository repo, IEntityMatcher matcher, EntityCollectionEventType eventType = EntityCollectionEventType.OnEntityAdded) {
            return new EntityRepositoryObserver(repo, matcher, eventType);
        }

        public static EntityWillBeRemovedEntityRepositoryObserver AddWillBeRemovedObserver(this EntityRepository repo, int index) {
            return new EntityWillBeRemovedEntityRepositoryObserver(repo, index);
        }
    }
}

