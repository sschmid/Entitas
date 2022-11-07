using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace My.Namespace
{
    public class EntityIndexNoContextComponent : IComponent
    {
        [EntityIndex]
        public string value;
    }
}
