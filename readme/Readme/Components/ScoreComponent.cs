using Entitas;
using Entitas.Plugins.Attributes;

[GameState, Unique]
public sealed class ScoreComponent : IComponent {

    public int value;
}
