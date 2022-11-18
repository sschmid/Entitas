using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Context("Test1"), Unique]
    public sealed class UniqueMyNamespaceComponent : IComponent
    {
        public string value;
    }
}
