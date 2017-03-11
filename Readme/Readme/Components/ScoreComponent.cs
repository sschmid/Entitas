using Entitas;
using Entitas.CodeGenerator.Api;

[GameState, Unique]
public sealed class ScoreComponent : IComponent {

    public int value;
}
