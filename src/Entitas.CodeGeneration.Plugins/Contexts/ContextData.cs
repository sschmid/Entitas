using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ContextData : CodeGeneratorData
    {
        public const string NameKey = "Context.Name";

        public string Name
        {
            get => (string)this[NameKey];
            set => this[NameKey] = value;
        }
    }
}
