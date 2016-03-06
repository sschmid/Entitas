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

    public static class PublicMemberInfoExtension {

        const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public;

        public static PublicMemberInfo[] GetPublicMemberInfos(this Type type) {

            var fieldInfos = getFieldInfos(type)
                .Select(info => new PublicMemberInfo(info.FieldType.FullName, info.Name, PublicMemberInfo.MemberType.Field));

            var propertyInfos = getPropertyInfos(type)
                .Select(info => new PublicMemberInfo(info.PropertyType.FullName, info.Name, PublicMemberInfo.MemberType.Property));

            return fieldInfos.Concat(propertyInfos).ToArray();
        }

        public static object PublicMemberClone(this object obj) {
            var type = obj.GetType();
            var clone = Activator.CreateInstance(type);

            var fieldInfos = getFieldInfos(type);
            for (int i = 0, fieldInfosLength = fieldInfos.Length; i < fieldInfosLength; i++) {
                var info = fieldInfos[i];
                info.SetValue(clone, info.GetValue(obj));
            }

            var propertyInfos = getPropertyInfos(type);
            for (int i = 0, fieldInfosLength = fieldInfos.Length; i < fieldInfosLength; i++) {
                var info = fieldInfos[i];
                info.SetValue(clone, info.GetValue(obj));
            }

            return clone;
        }

        static FieldInfo[] getFieldInfos(Type type) {
            return type.GetFields(BINDING_FLAGS);
        }

        static PropertyInfo[] getPropertyInfos(Type type) {
            return type
                .GetProperties(BINDING_FLAGS)
                .Where(info => info.CanRead && info.CanWrite)
                .ToArray();
        }
    }
}

