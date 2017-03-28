using Entitas;
using Entitas.CodeGenerator.Attributes;

[GameState, Unique]
public sealed class ScoreComponent : IComponent {

    public int value;
}
