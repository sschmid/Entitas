using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Context("Test1"), Context("Test2")]
    public sealed class MyNamespaceComponent : IComponent
    {
        public string value;
    }
}
