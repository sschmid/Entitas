using Jenny;
using Entitas.Plugins.Attributes;
using Entitas.Plugins;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class CleanupData : CodeGeneratorData
    {
        public const string CleanupModeKey = "Cleanup.Mode";

        public CleanupMode CleanupMode
        {
            get => (CleanupMode)this[CleanupModeKey];
            set => this[CleanupModeKey] = value;
        }

        public readonly ComponentData ComponentData;

        public CleanupData(CodeGeneratorData data) : base(data)
        {
            ComponentData = (ComponentData)data;
        }
    }
}
