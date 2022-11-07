using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins
{
    public class MemberDataComponentDataProvider : IComponentDataProvider
    {
        readonly string _componentInterface = typeof(IComponent).ToCompilableString();

        public void Provide(INamedTypeSymbol type, ComponentData data)
        {
            var isComponent = type.AllInterfaces.Any(i => i.ToCompilableString() == _componentInterface);
            var memberData = type.GetPublicMembers(isComponent)
                .Select(createMemberData)
                .ToArray();

            data.SetMemberData(memberData);
        }

        MemberData createMemberData(ISymbol member) =>
            new MemberData(member.PublicMemberType().ToCompilableString(), member.Name);
    }
}
