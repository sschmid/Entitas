using Entitas;

namespace Readme {

    public class Res {
        public const string redGem = null;
    }

    public static class ReadmeSnippets {
        static ReadmeSnippets() {
            new AnimatingComponent();
            new CoinsComponent();
            new GameBoardElementComponent();
            new HealthComponent();
            new InteractiveComponent();
            new MovableComponent();
            new PositionComponent();
            new ResourceComponent();
            new UIPositionComponent();
            new UserComponent();
        }


        public static Entity CreateRedGem(this Pool pool, int x, int y) {
            return pool.CreateEntity()
                       .IsGameBoardElement(true)
                       .IsMovable(true)
                       .AddPosition(x, y)
                       .AddResource(Res.redGem)
                       .IsInteractive(true);
        }


        static void moveSystem(Pool pool) {
            var entities = pool.GetEntities(Matcher.AllOf(Matcher.Move, Matcher.Position));
            foreach (var entity in entities) {
                var move = entity.move;
                var pos = entity.position;
                entity.ReplacePosition(pos.x, pos.y + move.speed);
            }
        }


        /*
         * 
         * Wiki
         * 
         * 
         */

        static void entityExample(Entity entity) {
            entity.AddPosition(3, 7);
            entity.AddHealth(100);
            entity.isMovable = true;

            entity.ReplacePosition(10, 100);
            entity.ReplaceHealth(entity.health.health - 1);
            entity.isMovable = false;

            entity.RemovePosition();

            var hasPos = entity.hasPosition;
            var movable = entity.isMovable;
        }

        static void poolExample() {
            // Pools.pool is kindly generated for you by the code generator
            var pool = Pools.pool;
            var entity = pool.CreateEntity();
            entity.isMovable = true;

            // Returns all entities having MovableComponent and PositionComponent.
            // Matchers are also generated for you.
            var entities = pool.GetEntities(Matcher.AllOf(Matcher.Movable, Matcher.Position));
            foreach (var e in entities) {
                // do something
            }
        }

        static void groupExample(Pool pool) {
            pool.GetGroup(Matcher.Position).GetEntities();

            // ----------------------------

            pool.GetGroup(Matcher.Position).OnEntityAdded += (group, entity, index, component) => {
                // Do something
            };
        }

        static void groupObserverExample(Pool pool) {
            var group = pool.GetGroup(Matcher.Position);
            var observer = group.CreateObserver(GroupEventType.OnEntityAdded);

            // ----------------------------
            foreach (var e in observer.collectedEntities) {
                // do something
            }
            observer.ClearCollectedEntities();


            // ----------------------------
            observer.Deactivate();
        }

        static void positionComponent(Entity e, PositionComponent component, int x, int y) {
            var pos = e.position;
            var has = e.hasPosition;

            e.AddPosition(x, y);
            e.ReplacePosition(x, y);
            e.RemovePosition();
        }

        #pragma warning disable
        static void userComponent(Pool pool, UserComponent component) {
            var e = pool.userEntity;
            var name = pool.user.name;
            var has = pool.hasUser;

            pool.SetUser("John", 42);
            pool.ReplaceUser("Max", 24);
            pool.RemoveUser();
        }

        static void movableComponent(Entity e) {
            var movable = e.isMovable;
            e.isMovable = true;
            e.isMovable = false;
        }

        static void animatingComponent(Pool pool) {
            var e = pool.animatingEntity;
            var isAnimating = pool.isAnimating;
            pool.isAnimating = true;
            pool.isAnimating = false;
        }
    }
}

