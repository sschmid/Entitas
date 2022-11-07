using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public interface IComponentDataProvider
    {
        void Provide(INamedTypeSymbol type, ComponentData data);
    }
}
