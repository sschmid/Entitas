using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Context(typeof(OtherContext))]
    [Unique]
    public sealed class UserComponent : IComponent
    {
        public string Name;
        public int Age;
    }
}
