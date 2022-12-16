using Jenny;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public class CleanupData : CodeGeneratorData
    {
        public const string CleanupModeKey = "Cleanup.Mode";

        public readonly ComponentData ComponentData;

        public CleanupData(CodeGeneratorData data, CleanupMode cleanupMode) : base(data)
        {
            ComponentData = (ComponentData)data;
            CleanupMode = cleanupMode;
        }

        public CleanupMode CleanupMode
        {
            get => (CleanupMode)this[CleanupModeKey];
            set => this[CleanupModeKey] = value;
        }
    }
}
