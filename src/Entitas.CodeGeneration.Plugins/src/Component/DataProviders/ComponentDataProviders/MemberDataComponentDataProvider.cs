using System;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;

namespace Entitas.CodeGeneration.Plugins
{
    public class MemberDataComponentDataProvider : IComponentDataProvider
    {
        public void Provide(Type type, ComponentData data)
        {
            var memberData = type.GetPublicMemberInfos()
                .Select(info => new MemberData(
                    info.Type.ToCompilableString(),
                    info.Name))
                .ToArray();

            data.SetMemberData(memberData);
        }
    }

    public static class MemberInfosComponentDataExtension
    {
        public const string COMPONENT_MEMBER_DATA = "Component.MemberData";
        public static MemberData[] GetMemberData(this ComponentData data) => (MemberData[])data[COMPONENT_MEMBER_DATA];
        public static void SetMemberData(this ComponentData data, MemberData[] memberInfos) => data[COMPONENT_MEMBER_DATA] = memberInfos;
    }
}
