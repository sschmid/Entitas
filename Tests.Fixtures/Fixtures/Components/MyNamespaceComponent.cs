using Entitas.Api;
using Entitas.CodeGenerator.Api;


namespace My.Namespace {

    [Context("Test"), Context("Test2")]
    public sealed class MyNamespaceComponent : IComponent {
        public string value;
    }
}
