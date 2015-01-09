using Entitas;

namespace Readme {
    public class ReadmeSnippets {
        public ReadmeSnippets() {
            new HealthComponent();
            new MovableComponent();
            new PositionComponent();
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

        void contextExample() {
            // Total components is kindly generated for you by the code generator
            var context = new Context(ComponentIds.TotalComponents);
            var entity = context.CreateEntity();
            entity.isMovable = true;

            // Returns all entities having MoveComponent and PositionComponent. Matchers are also generated for you.
            var entities = context.GetEntities(Matcher.AllOf(Matcher.Movable, Matcher.Position));
            foreach (var e in entities) {
                // do something
            }
        }

        void groupExample(Context context) {
            context.GetGroup(Matcher.Position).GetEntities();
        }

        void contextObserverExample(Context context) {
            var observer = context.CreateObserver(
                               Matcher.Position,
                               GroupEventType.OnEntityAdded
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

        void userComponent(Context context, UserComponent component) {
            var e = context.userEntity;
            var name = context.user.name;
            var has = context.hasUser;

            context.SetUser("John", 42);
            context.SetUser(component);

            context.ReplaceUser("Max", 24);

            context.RemoveUser();
        }

        void movableComponent(Entity e) {
            var movable = e.isMovable;
            e.isMovable = true;
            e.isMovable = false;
        }

        void animatingComponent(Context context) {
            var e = context.animatingEntity;
            var isAnimating = context.isAnimating;
            context.isAnimating = true;
            context.isAnimating = false;
        }
    }
}

