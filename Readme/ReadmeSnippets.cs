using Entitas;

namespace Readme {
    public class ReadmeSnippets {
        public ReadmeSnippets() {
            new AnimatingComponent();
            new CoinsComponent();
            new HealthComponent();
            new MovableComponent();
            new PositionComponent();
            new UIPositionComponent();
            new UserComponent();
        }

        void entityExample(Entity entity) {
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

        void poolExample() {
            // Total components is kindly generated for you by the code generator
            var pool = new Pool(ComponentIds.TotalComponents);
            var entity = pool.CreateEntity();
            entity.isMovable = true;

            // Returns all entities having MoveComponent and PositionComponent.
            // Matchers are also generated for you.
            var entities = pool.GetEntities(Matcher.AllOf(Matcher.Movable, Matcher.Position));
            foreach (var e in entities) {
                // do something
            }
        }

        void groupExample(Pool pool) {
            pool.GetGroup(Matcher.Position).GetEntities();
        }

        void groupObserverExample(Pool pool) {
            var group = pool.GetGroup(Matcher.Position);
            var observer = group.CreateObserver(GroupEventType.OnEntityAdded);

            // ----------------------------
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

        void userComponent(Pool pool, UserComponent component) {
            var e = pool.userEntity;
            var name = pool.user.name;
            var has = pool.hasUser;

            pool.SetUser("John", 42);
            pool.SetUser(component);

            pool.ReplaceUser("Max", 24);

            pool.RemoveUser();
        }

        void movableComponent(Entity e) {
            var movable = e.isMovable;
            e.isMovable = true;
            e.isMovable = false;
        }

        void animatingComponent(Pool pool) {
            var e = pool.animatingEntity;
            var isAnimating = pool.isAnimating;
            pool.isAnimating = true;
            pool.isAnimating = false;
        }
    }
}

