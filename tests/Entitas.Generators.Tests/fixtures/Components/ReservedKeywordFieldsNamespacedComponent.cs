using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class ReservedKeywordFieldsNamespacedComponent : IComponent
    {
        public string Namespace;
        public string Class;
        public string Public;
    }
}
