using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;

namespace Entitas.Blueprints {

    [Serializable]
    public class ComponentBlueprint {

        public int index;
        public string fullTypeName;
        public SerializableMember[] members;

        Type _type;
        Dictionary<string, PublicMemberInfo> _componentMembers;

        public ComponentBlueprint() {
        }

        public ComponentBlueprint(int index, IComponent component) {
            _type = component.GetType();
            _componentMembers = null;

            this.index = index;
            fullTypeName = _type.FullName;

            var memberInfos = _type.GetPublicMemberInfos();
            members = new SerializableMember[memberInfos.Length];
            for (int i = 0; i < memberInfos.Length; i++) {
                var info = memberInfos[i];
                members[i] = new SerializableMember(
                    info.Name, info.GetValue(component)
                );
            }
        }

        public IComponent CreateComponent(IEntity entity) {
            if (_type == null) {
                _type = fullTypeName.ToType();

                if (_type == null) {
                    throw new ComponentBlueprintException(
                        "Type '" + fullTypeName +
                        "' doesn't exist in any assembly!",
                        "Please check the full type name."
                    );
                }

                if (!_type.ImplementsInterface<IComponent>()) {
                    throw new ComponentBlueprintException(
                        "Type '" + fullTypeName +
                        "' doesn't implement IComponent!",
                        typeof(ComponentBlueprint).Name +
                        " only supports IComponent."
                    );
                }
            }

            var component = entity.CreateComponent(index, _type);

            if (_componentMembers == null) {
                var memberInfos = _type.GetPublicMemberInfos();
                _componentMembers = new Dictionary<string, PublicMemberInfo>(
                    memberInfos.Length
                );
                for (int i = 0; i < memberInfos.Length; i++) {
                    var info = memberInfos[i];
                    _componentMembers.Add(info.Name, info);
                }
            }

            for (int i = 0; i < members.Length; i++) {
                var member = members[i];

                PublicMemberInfo memberInfo;
                if (_componentMembers.TryGetValue(member.name, out memberInfo)) {
                    memberInfo.SetValue(component, member.value);
                } else {
                    Console.WriteLine(
                        "Could not find member '" + member.name +
                        "' in type '" + _type.FullName + "'!\n" +
                        "Only non-static public members are supported."
                    );
                }
            }

            return component;
        }
    }
}
