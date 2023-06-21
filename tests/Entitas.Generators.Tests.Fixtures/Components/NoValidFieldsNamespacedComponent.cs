using Entitas;

#pragma warning disable CS0169
#pragma warning disable CS0649

namespace MyFeature
{
    [MyApp.Main.Context]
    public sealed class NoValidFieldsNamespacedComponent : IComponent
    {
        public static string PublicStaticField;

        string _privateField;
        static string _privateStaticField;
    }
}
