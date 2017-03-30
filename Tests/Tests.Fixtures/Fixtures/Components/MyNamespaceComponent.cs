using Entitas;
using Entitas.CodeGenerator.Attributes;

namespace My.Namespace {

    [Context("Test"), Context("Test2")]
    public sealed class MyNamespaceComponent : IComponent {
        public string value;
    }
}
