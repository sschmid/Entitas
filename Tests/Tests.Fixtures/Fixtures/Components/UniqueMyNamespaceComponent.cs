using Entitas;
using Entitas.CodeGenerator.Api;

namespace My.Namespace {

    [Context("Test"), Unique]
    public sealed class UniqueMyNamespaceComponent : IComponent {
        public string value;
    }
}
