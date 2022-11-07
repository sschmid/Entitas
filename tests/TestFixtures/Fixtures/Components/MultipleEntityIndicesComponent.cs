using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    [Test, Test2]
    public class MultipleEntityIndicesComponent : IComponent
    {
        [EntityIndex]
        public string value;

        [EntityIndex]
        public string value2;
    }
}
