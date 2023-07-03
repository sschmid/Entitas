using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyOtherFeature
{
    [Context(typeof(LibraryContext))]
    public class HealthComponent : IComponent
    {
        public int Value;
    }
}
