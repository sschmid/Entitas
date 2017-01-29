using Entitas.Api;
using Entitas.CodeGenerator.Api;

namespace My.Namespace {

    [Test, Unique]
    public sealed class UniqueMyNamespaceComponent : IComponent {
        public string value;
    }
}
