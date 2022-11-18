using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.Plugins
{
    public class TemplatesConfig : AbstractConfigurableConfig
    {
        const string TemplatesKey = "Entitas.CodeGeneration.Plugins.Templates";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {TemplatesKey, "Plugins/Entitas/Templates"}
        };

        public string[] Templates
        {
            get => _preferences[TemplatesKey].FromCSV(true).ToArray();
            set => _preferences[TemplatesKey] = value.ToCSV(false, true);
        }
    }
}
