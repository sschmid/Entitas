using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Test1, Test2]
    public class MultipleEntityIndicesComponent : IComponent
    {
        [EntityIndex]
        public string value;

        [EntityIndex]
        public string value2;
    }
}
