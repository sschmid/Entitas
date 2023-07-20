#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext)), Cleanup(CleanupMode.DestroyEntity)]
public sealed class CleanupDestroyEntityComponent : IComponent { }
