using Entitas;

#pragma warning disable CS0649

namespace MyFeature
{
    [MyApp.Main.Context]
    partial class ReservedKeywordFieldsNamespacedComponent : IComponent
    {
        public string Namespace;
        public string Class;
        public string Public;
    }
}
