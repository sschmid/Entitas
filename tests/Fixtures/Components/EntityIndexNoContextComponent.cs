using Entitas;
using Entitas.Plugins.Attributes;

namespace My.Namespace
{
    public class EntityIndexNoContextComponent : IComponent
    {
        [EntityIndex]
        public string value;
    }
}
