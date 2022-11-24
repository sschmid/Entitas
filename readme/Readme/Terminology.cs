using System.Collections.Generic;
using Entitas;
using Entitas.Plugins.Attributes;
using UnityEngine;

// ReSharper disable UnusedVariable

public class Terminology
{
    void Createplayer(GameEntity entity)
    {
        entity.isPlayer = true;
        entity.AddPosition(Vector3.zero);
        entity.AddHealth(100);
    }

    void EntitasAPI(GameEntity entity)
    {
        var isPlayer = entity.isPlayer;
        var position = entity.position.Value;
        var health = entity.health.Value;

        entity.ReplaceHealth(50);
        entity.RemovePosition();
    }

    void UniqueComponents(GameContext context)
    {
        context.SetHighscore(100);
        context.ReplaceHighscore(200);
    }

    void MultipleContexts(Contexts contexts)
    {
        // works as expected
        contexts.game.CreateEntity().AddHealth(100);

        // doesn't compile, because HealthComponent is only in the game context
        // contexts.input.CreateEntity().AddHealth(100);
    }

    void HealthGroup(GameContext context)
    {
        var group = context.GetGroup(GameMatcher.Health);
    }

    void PositionHealthGroup(GameContext context)
    {
        var group = context.GetGroup(GameMatcher.AllOf(
            GameMatcher.Position,
            GameMatcher.Health)
        );
    }

    void NoneOfGroup(GameContext context)
    {
        var group = context.GetGroup(GameMatcher
            .AllOf(GameMatcher.Position)
            .NoneOf(GameMatcher.Health)
        );
    }

    void GroupIterate1(IGroup<GameEntity> group)
    {
        foreach (var entity in group)
        {
            // do sth
        }
    }

    void GroupIterate2(IGroup<GameEntity> group)
    {
        foreach (var entity in group.GetEntities())
        {
            // do sth
        }
    }

    void GroupIterate3(IGroup<GameEntity> group)
    {
        var buffer = new List<GameEntity>();
        foreach (var entity in group.GetEntities(buffer))
        {
            // do sth
        }
    }
}

/**
 *
 */
public sealed class PositionComponent : IComponent
{
    public Vector3 Value;
}

public sealed class HealthComponent : IComponent
{
    public int Value;
}

public sealed class PlayerComponent : IComponent { }

public sealed class GameOverComponent : IComponent { }

[Unique]
public sealed class HighscoreComponent : IComponent
{
    public int Value;
}

[Input]
public sealed class InputComponent : IComponent
{
    public Vector2 Position;
}
