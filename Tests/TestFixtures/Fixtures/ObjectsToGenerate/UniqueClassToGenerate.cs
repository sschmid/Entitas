using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    [Context("Test"), Context("Test2"), Unique]
    public sealed class UniqueClassToGenerate
    {
        public string value;
    }
}
