using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class ShouldGenerateComponentComponentDataProvider : IComponentDataProvider
    {
        public void Provide(INamedTypeSymbol type, ComponentData data)
        {
            var shouldGenerateComponent = !type.AllInterfaces.Any(i => i.ToCompilableString() == typeof(IComponent).ToCompilableString());
            data.ShouldGenerateComponent(shouldGenerateComponent);
            if (shouldGenerateComponent)
                data.SetObjectTypeName(type.ToCompilableString());
        }
    }
}
