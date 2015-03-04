using Entitas.CodeGenerator;

namespace Entitas.Debug {
    [DontGenerate(false)]
    public class DebugComponent : IComponent {
        public EntityDebugBehaviour debugBehaviour;
    }
}