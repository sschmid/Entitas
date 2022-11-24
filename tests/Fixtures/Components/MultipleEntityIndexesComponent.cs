using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Test1, Test2]
    public class MultipleEntityIndexesComponent : IComponent
    {
        [EntityIndex]
        public string Value;

        [EntityIndex]
        public string Value2;
    }
}
