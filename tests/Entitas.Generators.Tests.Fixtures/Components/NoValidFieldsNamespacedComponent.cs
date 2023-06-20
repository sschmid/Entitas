using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context]
    partial class NoValidFieldsNamespacedComponent : IComponent
    {
        public static string PublicStaticField;
        string _privateField;
        static string _privateStaticField;
    }
}
