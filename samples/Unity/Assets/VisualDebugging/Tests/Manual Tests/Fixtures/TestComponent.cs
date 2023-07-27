using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext)), Context(typeof(InputContext))]
public sealed class TestComponent : IComponent { }
