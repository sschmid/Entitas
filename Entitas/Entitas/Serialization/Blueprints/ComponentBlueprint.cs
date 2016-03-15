using System;
using System.Collections.Generic;
using System.Linq;
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

        public IComponent CreateComponent() {
            var type = getComponentType(fullTypeName);
            var component = (IComponent)Activator.CreateInstance(type);
            var componentMembers = type.GetPublicMemberInfos().ToDictionary(info => info.name);

            for (int i = 0, membersLength = members.Length; i < membersLength; i++) {
                var member = members[i];

                PublicMemberInfo memberInfo;
                if (!componentMembers.TryGetValue(member.name, out memberInfo)) {
                    throw new ComponentBlueprintException("Could not find member '" + member.name + "' in Type '" + type.FullName + "'!", "Only non-static public members are supported.");
                }

                memberInfo.SetValue(component, member.value);
            }

            return component;
        }

        static Type getComponentType(string fullTypeName) {
            var componentType = Type.GetType(fullTypeName);
            if (componentType != null) {
                return componentType;
            }
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                componentType = assembly.GetType(fullTypeName);
                if (componentType != null) {
                    return componentType;
                }
            }

            throw new ComponentBlueprintException("Type '" + fullTypeName + "' doesn't exist in any assembly!",
                "Please check the full type name.");
        }
    }

    public class ComponentBlueprintException : EntitasException {
        public ComponentBlueprintException(string message, string hint) : base(message, hint) {
        }
    }
}
