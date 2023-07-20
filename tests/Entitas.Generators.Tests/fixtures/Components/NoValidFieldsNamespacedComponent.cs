#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

#pragma warning disable CS0169

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class NoValidFieldsNamespacedComponent : IComponent
    {
        public static string PublicStaticField;

        string _privateField;
        static string _privateStaticField;
    }
}
