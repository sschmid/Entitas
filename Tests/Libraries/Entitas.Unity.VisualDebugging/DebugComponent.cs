using Entitas.CodeGenerator;

namespace Entitas.Unity.VisualDebugging {
    [DontGenerate(false)]
    public class DebugComponent : IComponent {
        public EntityDebugBehaviour debugBehaviour;
    }
}