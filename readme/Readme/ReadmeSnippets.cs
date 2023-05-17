using Entitas;
using UnityEngine;
using static GameMatcher;

// ReSharper disable UnusedVariable

namespace Readme
{
    public sealed class MoveSystem : IExecuteSystem
    {
        readonly IGroup<GameEntity> _group;

        public MoveSystem(GameContext context)
        {
            _group = context.GetGroup(AllOf(Position, Velocity));
        }

        public void Execute()
        {
            foreach (var e in _group.GetEntities())
                e.ReplacePosition(e.position.value + e.velocity.value);
        }
    }

    public static class ReadmeSnippets
    {
        public static void CreateEntity(this GameContext context, Vector3 position)
        {
            var entity = context.CreateEntity();
            entity.AddPosition(Vector3.zero);
            entity.AddVelocity(Vector3.forward);
            entity.AddAsset("Player");
        }

        /*
         *
         * Wiki
         *
         *
         */

        static void EntityExample(GameEntity entity)
        {
            entity.AddPosition(new Vector3(1, 2, 3));
            entity.AddHealth(100);
            entity.isMovable = true;

            entity.ReplacePosition(new Vector3(10, 20, 30));
            entity.ReplaceHealth(entity.health.value - 1);
            entity.isMovable = false;

            entity.RemovePosition();

            var hasPos = entity.hasPosition;
            var movable = entity.isMovable;
        }

        static void ContextExample()
        {
            // contexts.game is kindly generated for you by the code generator
            var context = Contexts.sharedInstance.game;
            var entity = context.CreateEntity();
            entity.isMovable = true;

            // Returns all entities having MovableComponent and PositionComponent.
            // Matchers are also generated for you.
            var entities = context.GetEntities(AllOf(
                Movable,
                Position)
            );
            foreach (var e in entities)
            {
                // do something
            }
        }

        static void GroupExample(GameContext context)
        {
            context.GetGroup(Position).GetEntities();

            // ----------------------------

            context.GetGroup(Position).OnEntityAdded += (group, entity, index, component) =>
            {
                // Do something
            };
        }

        static void CollectorExample(GameContext context)
        {
            var group = context.GetGroup(Position);
            var collector = group.CreateCollector(GroupEvent.Added);

            // ----------------------------
            foreach (var e in collector.collectedEntities)
            {
                // do something
            }

            collector.ClearCollectedEntities();


            // ----------------------------
            collector.Deactivate();
        }

        static void PositionComponent(GameEntity e, PositionComponent component, Vector3 position, Vector3 newPosition)
        {
            var pos = e.position;
            var has = e.hasPosition;

            e.AddPosition(position);
            e.ReplacePosition(newPosition);
            e.RemovePosition();
        }

        static void UserComponent(GameContext context, UserComponent component)
        {
            var entity = context.userEntity;
            var name = context.user.name;
            var has = context.hasUser;

            context.SetUser("John", 42);
            context.ReplaceUser("Max", 24);
            context.RemoveUser();
        }

        static void MovableComponent(GameEntity e)
        {
            var movable = e.isMovable;
            e.isMovable = true;
            e.isMovable = false;
        }

        static void AnimatingComponent(GameContext context)
        {
            var e = context.animatingEntity;
            var isAnimating = context.isAnimating;
            context.isAnimating = true;
            context.isAnimating = false;
        }
    }
}
