using Entitas;
using Entitas.CodeGeneration.Attributes;

[GameState, Unique]
public sealed class ScoreComponent : IComponent {

    public int value;
}
