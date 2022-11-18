using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.Plugins
{
    public class ContextConfig : AbstractConfigurableConfig
    {
        const string ContextsKey = "Entitas.CodeGeneration.Plugins.Contexts";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {ContextsKey, "Game, Input"}
        };

        public string[] Contexts
        {
            get => _preferences[ContextsKey].FromCSV(true).ToArray();
            set => _preferences[ContextsKey] = value.ToCSV(false, true);
        }
    }
}
