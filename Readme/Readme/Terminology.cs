using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

public class Terminology {

    void createplayer(GameEntity entity) {
        entity.isPlayer = true;
        entity.AddPosition(Vector3.zero);
        entity.AddHealth(100);
    }

    void entitasAPI(GameEntity entity) {
        var isPlayer = entity.isPlayer;
        var position = entity.position.value;
        var health = entity.health.value;

        entity.ReplaceHealth(50);
        entity.RemovePosition();
    }

    void uniqueComponents(GameContext context) {
        context.SetHighscore(100);
        context.ReplaceHighscore(200);
    }

    void multipleContexts(Contexts contexts) {
        // works as expected
        contexts.game.CreateEntity().AddHealth(100);

        // doens't complile, because HealthComponent is only in the gmae context
        // contexts.input.CreateEntity().AddHealth(100);
    }

    void healthGroup(GameContext context) {
        var group = context.GetGroup(GameMatcher.Health);
    }

    void positionHealthGroup(GameContext context) {
        var group = context.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Health));
    }

    void noneOfGroup(GameContext context) {
        var group = context.GetGroup(GameMatcher.AllOf(GameMatcher.Position).NoneOf(GameMatcher.Health));
    }

    void groupIterate1(IGroup<GameEntity> group) {
        foreach (var e in group) {
            // do sth
        }
    }

    void groupIterate2(IGroup<GameEntity> group) {
        foreach (var e in group.GetEntities()) {
            // do sth
        }
    }

    void groupIterate3(IGroup<GameEntity> group) {
var buffer = new List<GameEntity>();
foreach (var e in group.GetEntities(buffer)) {
    // do sth
}
    }

}

/**
 *
 */

public sealed class PositionComponent : IComponent {
    public Vector3 value;
}

public sealed class HealthComponent : IComponent {
    public int value;
}

public sealed class PlayerComponent : IComponent {
}

public sealed class GameOverComponent : IComponent {
}

[Unique]
public sealed class HighscoreComponent : IComponent {
    public int value;
}

[Input]
public sealed class InputComponent : IComponent {
    public Vector2 position;
}
