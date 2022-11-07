using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    [Context("Test")]
    public interface InterfaceToGenerate
    {
        string value { get; set; }
    }
}
