using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins
{
    public class TemplatesConfig : AbstractConfigurableConfig
    {
        const string TEMPLATES_KEY = "Entitas.CodeGeneration.Plugins.Templates";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {TEMPLATES_KEY, "Plugins/Entitas/Templates"}
        };

        readonly bool _minified;
        readonly bool _removeEmptyEntries;

        public TemplatesConfig() : this(false, true) { }

        public TemplatesConfig(bool minified, bool removeEmptyEntries)
        {
            _minified = minified;
            _removeEmptyEntries = removeEmptyEntries;
        }

        public string[] templates
        {
            get => _preferences[TEMPLATES_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[TEMPLATES_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }
    }
}
