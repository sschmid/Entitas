using Entitas;

namespace Readme {
    public class ReadmeSnippets {
        public ReadmeSnippets() {
            new HealthComponent();
            new MovableComponent();
            new PositionComponent();
            new UserComponent();
        }

        void entity(Entity entity) {
            entity.AddPosition(0, 0, 0);
            entity.AddHealth(100);
            entity.isMovable = true;

            entity.ReplacePosition(10, 100, -30);
            entity.ReplaceHealth(entity.health.health - 1);
            entity.isMovable = false;

            entity.RemovePosition();

            var hasPos = entity.hasPosition;
            var movable = entity.isMovable;
        }

        void entityRepository() {
            // Total components is kindly generated for you by the code generator
            var repo = new EntityRepository(ComponentIds.TotalComponents);
            var entity = repo.CreateEntity();
            entity.isMovable = true;

            // Returns all entities having MoveComponent. Matcher.Movable is also generated for you.
            var entities = repo.GetEntities(Matcher.Movable);
            foreach (var e in entities) {
                // do something
            }
        }

        void entityCollection(EntityRepository repo) {
            repo.GetCollection(Matcher.Position).GetEntities();
        }

        void entityRepositoryObserver(EntityRepository repo) {
            var observer = repo.CreateObserver(
                               Matcher.Position,
                               EntityCollectionEventType.OnEntityAdded
                           );

            var entities = observer.collectedEntities;
            foreach (var e in entities) {
                // do something
            }
            observer.ClearCollectedEntites();


            // ----------------------------
            observer.Deactivate();
        }

        void positionComponent(Entity e, PositionComponent component, int x, int y, int z) {
            var pos = e.position;
            var has = e.hasPosition;

            e.AddPosition(x, y, z);
            e.AddPosition(component);

            e.ReplacePosition(x, y, z);

            e.RemovePosition();
        }

        void userComponent(EntityRepository repo, UserComponent component) {
            var e = repo.userEntity;
            var name = repo.user.name;
            var has = repo.hasUser;

            repo.SetUser("John", 42);
            repo.SetUser(component);

            repo.ReplaceUser("Max", 24);

            repo.RemoveUser();
        }

        void movableComponent(Entity e) {
            var movable = e.isMovable;
            e.isMovable = true;
            e.isMovable = false;
        }

        void animatingComponent(EntityRepository repo) {
            var e = repo.animatingEntity;
            var isAnimating = repo.isAnimating;
            repo.isAnimating = true;
            repo.isAnimating = false;
        }
    }
}

