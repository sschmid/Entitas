using System.Collections.Generic;
using System.Linq;
using Jenny;
using Jenny.Plugins;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using Microsoft.CodeAnalysis;

namespace Entitas.Plugins
{
    public class CleanupDataProvider : IDataProvider, IConfigurable, ICachable
    {
        public string Name => "Cleanup (Roslyn)";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties => _projectPathConfig.DefaultProperties;

        public Dictionary<string, object> ObjectCache { get; set; }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();

        readonly INamedTypeSymbol[] _symbols;

        readonly string _componentInterface = typeof(IComponent).ToCompilableString();

        Preferences _preferences;
        ComponentDataProvider _componentDataProvider;

        public CleanupDataProvider() : this(null) { }

        public CleanupDataProvider(INamedTypeSymbol[] symbols) => _symbols = symbols;

        public void Configure(Preferences preferences)
        {
            _preferences = preferences;
            _projectPathConfig.Configure(preferences);
        }

        ProjectParser GetProjectParser()
        {
            var key = typeof(ProjectParser).FullName;
            if (!ObjectCache.TryGetValue(key, out var projectParser))
            {
                projectParser = new ProjectParser(_projectPathConfig.ProjectPath);
                ObjectCache.Add(key, projectParser);
            }

            return (ProjectParser)projectParser;
        }

        public CodeGeneratorData[] GetData()
        {
            var symbols = _symbols ?? GetProjectParser().GetTypes();

            var cleanupTypes = symbols
                .Where(symbol => !symbol.IsAbstract)
                .Where(symbol => symbol.AllInterfaces.Any(i => i.ToCompilableString() == _componentInterface))
                .Where(symbol => symbol.GetAttribute<CleanupAttribute>() != null)
                .ToArray();

            var cleanupLookup = cleanupTypes.ToDictionary(
                symbol => symbol.ToCompilableString(),
                symbol => (CleanupMode)symbol.GetAttribute<CleanupAttribute>().ConstructorArguments[0].Value);

            _componentDataProvider = new ComponentDataProvider(cleanupTypes);
            _componentDataProvider.Configure(_preferences);

            // ReSharper disable once CoVariantArrayConversion
            return _componentDataProvider.GetData()
                .Where(data => !((ComponentData)data).Type.RemoveComponentSuffix().HasListenerSuffix())
                .Select(data => new CleanupData(data, cleanupLookup[((ComponentData)data).Type]))
                .ToArray();
        }
    }
}
