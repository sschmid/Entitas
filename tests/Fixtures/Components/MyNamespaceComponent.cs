using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Context("Test1")]
    public sealed class MyNamespaceComponent : IComponent
    {
        public string Value;
    }
}
