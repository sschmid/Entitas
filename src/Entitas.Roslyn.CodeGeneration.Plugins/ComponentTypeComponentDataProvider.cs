using DesperateDevs.Roslyn;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class ComponentTypeComponentDataProvider : IComponentDataProvider
    {
        public void Provide(INamedTypeSymbol type, ComponentData data)
        {
            data.SetTypeName(type.ToCompilableString());
        }
    }
}
