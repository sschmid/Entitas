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


        public static GameEntity CreateRedGem(this GameContext context, int x, int y) {
            var entity = context.CreateEntity();
            entity.isGameBoardElement = true;
            entity.isMovable = true;
            entity.AddPosition(x, y);
            entity.AddResource(Res.redGem);
            entity.isInteractive = true;
            return entity;
        }


        static void moveSystem(GameContext context) {
            var entities = context.GetEntities(Matcher<GameEntity>.AllOf(GameMatcher.Move, GameMatcher.Position));
            foreach(var entity in entities) {
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

        static void entityExample(GameEntity entity) {
            entity.AddPosition(3, 7);
            entity.AddHealth(100);
            entity.isMovable = true;

            entity.ReplacePosition(10, 100);
            entity.ReplaceHealth(entity.health.value - 1);
            entity.isMovable = false;

            entity.RemovePosition();

            var hasPos = entity.hasPosition;
            var movable = entity.isMovable;
        }

        static void contextExample() {
            // contexts.context is kindly generated for you by the code generator
            var contexts = Contexts.sharedInstance; 
            var context = contexts.game;
            var entity = context.CreateEntity();
            entity.isMovable = true;

            // Returns all entities having MovableComponent and PositionComponent.
            // Matchers are also generated for you.
            var entities = context.GetEntities(Matcher<GameEntity>.AllOf(GameMatcher.Movable, GameMatcher.Position));
            foreach(var e in entities) {
                // do something
            }
        }

        static void groupExample(GameContext context) {
            context.GetGroup(GameMatcher.Position).GetEntities();

            // ----------------------------

            context.GetGroup(GameMatcher.Position).OnEntityAdded += (group, entity, index, component) => {
                // Do something
            };
        }

        static void collectorExample(GameContext context) {
            var group = context.GetGroup(GameMatcher.Position);
            var collector = group.CreateCollector(GroupEvent.Added);

            // ----------------------------
            foreach(var e in collector.collectedEntities) {
                // do something
            }
            collector.ClearCollectedEntities();


            // ----------------------------
            collector.Deactivate();
        }

        static void positionComponent(GameEntity e, PositionComponent component, int x, int y) {
            var pos = e.position;
            var has = e.hasPosition;

            e.AddPosition(x, y);
            e.ReplacePosition(x, y);
            e.RemovePosition();
        }

        #pragma warning disable
        static void userComponent(GameContext context, UserComponent component) {
            var e = context.userEntity;
            var name = context.user.name;
            var has = context.hasUser;

            context.SetUser("John", 42);
            context.ReplaceUser("Max", 24);
            context.RemoveUser();
        }

        static void movableComponent(GameEntity e) {
            var movable = e.isMovable;
            e.isMovable = true;
            e.isMovable = false;
        }

        static void animatingComponent(GameContext context) {
            var e = context.animatingEntity;
            var isAnimating = context.isAnimating;
            context.isAnimating = true;
            context.isAnimating = false;
        }
    }
}
