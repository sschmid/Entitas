using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins
{
    public class AssembliesConfig : AbstractConfigurableConfig
    {
        const string ASSEMBLIES_KEY = "Entitas.CodeGeneration.Plugins.Assemblies";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {ASSEMBLIES_KEY, "Library/ScriptAssemblies/Assembly-CSharp.dll"}
        };

        readonly bool _minified;
        readonly bool _removeEmptyEntries;

        public AssembliesConfig() : this(false, true) { }

        public AssembliesConfig(bool minified, bool removeEmptyEntries)
        {
            _minified = minified;
            _removeEmptyEntries = removeEmptyEntries;
        }

        public string[] assemblies
        {
            get => _preferences[ASSEMBLIES_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[ASSEMBLIES_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }
    }
}
