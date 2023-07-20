#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

#pragma warning disable CS0649

namespace MyFeature
{
    [Context(typeof(MainContext))]
    class NonPublicComponent : IComponent
    {
        public string Value;
    }
}
