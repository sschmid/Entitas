using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    [Context("Test"), Unique]
    public sealed class UniqueMyNamespaceFlagComponent : IComponent { }
}
