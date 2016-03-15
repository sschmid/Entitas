using System;
using Entitas.Serialization;

namespace Entitas.Serialization.Blueprints {

    [Serializable]
    public struct ComponentBlueprint {
        public int index;
        public string fullTypeName;
        public SerializableMember[] members;

        public ComponentBlueprint(int index, IComponent component) {
            var type = component.GetType();
            this.index = index;
            this.fullTypeName = type.FullName;

            var memberInfos = type.GetPublicMemberInfos();
            members = new SerializableMember[memberInfos.Length];
            for (int i = 0, memberInfosLength = memberInfos.Length; i < memberInfosLength; i++) {
                var info = memberInfos[i];
                members[i] = new SerializableMember(info.name, info.GetValue(component));
            }
        }
    }
}
