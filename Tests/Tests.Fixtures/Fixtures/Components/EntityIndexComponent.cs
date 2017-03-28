using Entitas;
using Entitas.CodeGenerator.Attributes;

namespace My.Namespace {

    [Test, Test2]
    public class EntityIndexComponent : IComponent {

        [EntityIndex]
        public string value;
    }
}
