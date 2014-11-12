namespace Entitas {
    public static class EntityRepositoryExtension {
        public static Entity[] GetEntities(this EntityRepository repo, IEntityMatcher matcher) {
            return repo.GetCollection(matcher).GetEntities();
        }

        public static EntityRepositoryObserver CreateObserver(this EntityRepository repo, IEntityMatcher matcher, EntityCollectionEventType eventType = EntityCollectionEventType.OnEntityAdded) {
            return new EntityRepositoryObserver(repo, matcher, eventType);
        }

        public static EntityWillBeRemovedEntityRepositoryObserver CreateWillBeRemovedObserver(this EntityRepository repo, AllOfEntityMatcher matcher) {
            return new EntityWillBeRemovedEntityRepositoryObserver(repo, matcher);
        }
    }
}

