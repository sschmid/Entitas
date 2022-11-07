using Jenny;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class CleanupData : CodeGeneratorData
    {
        public const string CLEANUP_MODE = "Cleanup.Mode";

        public CleanupMode cleanupMode
        {
            get => (CleanupMode)this[CLEANUP_MODE];
            set => this[CLEANUP_MODE] = value;
        }

        public ComponentData componentData => _componentData;

        readonly ComponentData _componentData;

        public CleanupData(CodeGeneratorData data) : base(data)
        {
            _componentData = (ComponentData)data;
        }
    }
}
