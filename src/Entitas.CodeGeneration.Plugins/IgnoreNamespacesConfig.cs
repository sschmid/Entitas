using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins
{
    public class IgnoreNamespacesConfig : AbstractConfigurableConfig
    {
        public const string IgnoreNamespacesKey = "Entitas.CodeGeneration.Plugins.IgnoreNamespaces";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {IgnoreNamespacesKey, "false"}
        };

        public bool IgnoreNamespaces
        {
            get => _preferences[IgnoreNamespacesKey] == "true";
            set => _preferences[IgnoreNamespacesKey] = value.ToString();
        }
    }
}
