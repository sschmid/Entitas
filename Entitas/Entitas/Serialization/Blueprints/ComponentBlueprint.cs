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

        Type _type;

        public ComponentBlueprint(int index, IComponent component) {
            _type = component.GetType();
            this.index = index;
            this.fullTypeName = _type.FullName;

            var memberInfos = _type.GetPublicMemberInfos();
            members = new SerializableMember[memberInfos.Length];
            for (int i = 0, memberInfosLength = memberInfos.Length; i < memberInfosLength; i++) {
                var info = memberInfos[i];
                members[i] = new SerializableMember(info.name, info.GetValue(component));
            }
        }

        public IComponent CreateComponent() {
            if (_type == null) {
                _type = fullTypeName.ToType();

                if (_type == null) {
                    throw new ComponentBlueprintException("Type '" + fullTypeName + "' doesn't exist in any assembly!",
                        "Please check the full type name.");
                }
                
                if (!_type.ImplementsInterface<IComponent>()) {
                    throw new ComponentBlueprintException("Type '" + fullTypeName + "' doesn't implement IComponent!",
                        typeof(ComponentBlueprint).Name + " only supports IComponent.");
                }
            }

            var component = (IComponent)Activator.CreateInstance(_type);
            var componentMembers = _type.GetPublicMemberInfos().ToDictionary(info => info.name);

            for (int i = 0, membersLength = members.Length; i < membersLength; i++) {
                var member = members[i];

                PublicMemberInfo memberInfo;
                if (!componentMembers.TryGetValue(member.name, out memberInfo)) {
                    throw new ComponentBlueprintException("Could not find member '" + member.name + "' in type '" + _type.FullName + "'!", "Only non-static public members are supported.");
                }

                memberInfo.SetValue(component, member.value);
            }

            return component;
        }
    }

    public class ComponentBlueprintException : EntitasException {
        public ComponentBlueprintException(string message, string hint) : base(message, hint) {
        }
    }
}
