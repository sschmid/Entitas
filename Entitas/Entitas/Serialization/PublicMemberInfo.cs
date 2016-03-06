using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas.Serialization {

    public struct PublicMemberInfo {

        public enum MemberType {
            Undefined,
            Field,
            Property
        }

        public readonly string fullTypeName;
        public readonly string name;
        public readonly MemberType memberType;

        public PublicMemberInfo(string fullTypeName, string name, MemberType memberType) {
            this.fullTypeName = fullTypeName;
            this.name = name;
            this.memberType = memberType;
        }
    }

    public static class ComponentMemberInfoExtension {

        public static PublicMemberInfo[] GetPublicMemberInfos(this Type componentType) {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = componentType.GetFields(bindingFlags)
                .Select(fieldInfo => new PublicMemberInfo(fieldInfo.FieldType.FullName, fieldInfo.Name, PublicMemberInfo.MemberType.Field));

            var propertyInfos = componentType
                .GetProperties(bindingFlags)
                .Where(propertyInfo  => propertyInfo.CanRead && propertyInfo.CanWrite)
                .Select(propertyInfo => new PublicMemberInfo(propertyInfo.PropertyType.FullName, propertyInfo.Name, PublicMemberInfo.MemberType.Property));

            return fieldInfos.Concat(propertyInfos).ToArray();
        }
    }
}

