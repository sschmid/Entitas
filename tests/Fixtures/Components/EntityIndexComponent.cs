using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    [Test1, Test2]
    public class EntityIndexComponent : IComponent
    {
        [EntityIndex]
        public string Value;
    }
}
