#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(LibraryContext))]
    public sealed class ContextFromDifferentAssemblyNamespacedComponent : IComponent { }
}
