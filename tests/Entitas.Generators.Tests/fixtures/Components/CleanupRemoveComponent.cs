#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext)), Cleanup(CleanupMode.RemoveComponent)]
public sealed class CleanupRemoveComponent : IComponent { }
