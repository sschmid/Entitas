using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins
{
    public class ContextNamesConfig : AbstractConfigurableConfig
    {
        const string CONTEXTS_KEY = "Entitas.CodeGeneration.Plugins.Contexts";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {CONTEXTS_KEY, "Game, Input"}
        };

        readonly bool _minified;
        readonly bool _removeEmptyEntries;

        public ContextNamesConfig() : this(false, true) { }

        public ContextNamesConfig(bool minified, bool removeEmptyEntries)
        {
            _minified = minified;
            _removeEmptyEntries = removeEmptyEntries;
        }

        public string[] contextNames
        {
            get => _preferences[CONTEXTS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[CONTEXTS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }
    }
}
