using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    [Context("Test1"), Context("Test2")]
    public sealed class ClassToGenerate
    {
        public string value;
    }
}
