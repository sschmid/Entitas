using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins
{
    public class AssembliesConfig : AbstractConfigurableConfig
    {
        const string AssembliesKey = "Entitas.CodeGeneration.Plugins.Assemblies";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {AssembliesKey, "Library/ScriptAssemblies/Assembly-CSharp.dll"}
        };

        public string[] Assemblies
        {
            get => _preferences[AssembliesKey].FromCSV(true).ToArray();
            set => _preferences[AssembliesKey] = value.ToCSV(false, true);
        }
    }
}
