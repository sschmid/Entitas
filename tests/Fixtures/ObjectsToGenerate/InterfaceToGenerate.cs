using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Context("Test1")]
    public interface InterfaceToGenerate
    {
        string Value { get; set; }
    }
}
