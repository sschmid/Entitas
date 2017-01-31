using Entitas.CodeGenerator.Api;

namespace My.Namespace {

    [Context("Test")]
    public interface InterfaceToGenerate {
        string value { get; set; }
    }
}
