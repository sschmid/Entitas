using System;
using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGenerator {

    public class MemberInfosComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetMemberInfos(type.GetPublicMemberInfos());
        }
    }

    public static class MemberInfosComponentDataProviderExtension {

        public const string COMPONENT_MEMBER_INFOS = "component_memberInfos";

        public static List<PublicMemberInfo> GetMemberInfos(this ComponentData data) {
            return (List<PublicMemberInfo>)data[COMPONENT_MEMBER_INFOS];
        }

        public static void SetMemberInfos(this ComponentData data, List<PublicMemberInfo> memberInfos) {
            data[COMPONENT_MEMBER_INFOS] = memberInfos;
        }
    }
}
