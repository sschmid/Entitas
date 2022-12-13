using System.Collections.Generic;
using System.Linq;
using Jenny;
using DesperateDevs.Serialization;

namespace Entitas.Plugins
{
    public class ContextDataProvider : IDataProvider, IConfigurable
    {
        public string Name => "Context";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties => _contextConfig.DefaultProperties;

        readonly ContextConfig _contextConfig = new ContextConfig();

        public void Configure(Preferences preferences)
        {
            _contextConfig.Configure(preferences);
        }

        // ReSharper disable once CoVariantArrayConversion
        public CodeGeneratorData[] GetData() => _contextConfig.Contexts
            .Select(context =>
            {
                var name = context.RemoveContextSuffix();
                return new ContextData
                {
                    Name = name,
                    Type = context.AddContextSuffix(),
                    EntityType = name.AddEntitySuffix()
                };
            }).ToArray();
    }
}
