using System.Linq;
using DesperateDevs.Roslyn;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class FlagPrefixComponentDataProvider : IComponentDataProvider
    {
        public void Provide(INamedTypeSymbol type, ComponentData data)
        {
            data.SetFlagPrefix(getCustomComponentPrefix(type));
        }

        string getCustomComponentPrefix(INamedTypeSymbol type)
        {
            var attr = type.GetAttribute<FlagPrefixAttribute>();
            return attr == null ? "is" : (string)attr.ConstructorArguments.First().Value;
        }
    }
}
