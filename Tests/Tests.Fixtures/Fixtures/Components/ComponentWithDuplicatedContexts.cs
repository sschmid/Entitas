using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace My.Namespace {

    [Context("Test"), Context("Test")]
    public sealed class ComponentWithDuplicatedContexts : IComponent {
        public string value;
    }
}
