using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    [Context("Test1"), Context("Test2"), Unique]
    public sealed class UniqueClassToGenerate
    {
        public string value;
    }
}
