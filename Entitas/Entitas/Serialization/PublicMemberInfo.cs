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
        public readonly object value;

        public PublicMemberInfo(string fullTypeName, string name, MemberType memberType, object value = null) {
            this.fullTypeName = fullTypeName;
            this.name = name;
            this.memberType = memberType;
            this.value = value;
        }
    }

    public static class PublicMemberInfoExtension {

        const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public;

        public static PublicMemberInfo[] GetPublicMemberInfos(this Type type, bool typeToCompilableString = false) {

            var fieldInfos = getFieldInfos(type)
                .Select(info => new PublicMemberInfo(
                    typeToCompilableString ? info.FieldType.ToCompilableString() : info.FieldType.FullName,
                    info.Name, PublicMemberInfo.MemberType.Field
                ));

            var propertyInfos = getPropertyInfos(type)
                .Select(info => new PublicMemberInfo(
                    typeToCompilableString ? info.PropertyType.ToCompilableString() : info.PropertyType.FullName,
                    info.Name, PublicMemberInfo.MemberType.Property));

            return fieldInfos.Concat(propertyInfos).ToArray();
        }

        public static PublicMemberInfo[] GetPublicMemberInfos(this object obj, bool typeToCompilableString = false) {
            var type = obj.GetType();
            var fieldInfos = getFieldInfos(type)
                .Select(info => new PublicMemberInfo(
                    typeToCompilableString ? info.FieldType.ToCompilableString() : info.FieldType.FullName,
                    info.Name, PublicMemberInfo.MemberType.Field,
                    info.GetValue(obj)
                ));

            var propertyInfos = getPropertyInfos(type)
                .Select(info => new PublicMemberInfo(
                    typeToCompilableString ? info.PropertyType.ToCompilableString() : info.PropertyType.FullName,
                    info.Name, PublicMemberInfo.MemberType.Property,
                    info.GetValue(obj, null)
                ));

            return fieldInfos.Concat(propertyInfos).ToArray();
        }

        public static object PublicMemberClone(this object obj) {
            var clone = Activator.CreateInstance(obj.GetType());
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static void CopyPublicMemberValues(this object obj, object target) {
            var type = obj.GetType();
            var fieldInfos = getFieldInfos(type);
            for (int i = 0, fieldInfosLength = fieldInfos.Length; i < fieldInfosLength; i++) {
                var info = fieldInfos[i];
                info.SetValue(target, info.GetValue(obj));
            }

            var propertyInfos = getPropertyInfos(type);
            for (int i = 0, propertyInfosLength = propertyInfos.Length; i < propertyInfosLength; i++) {
                var info = propertyInfos[i];
                info.SetValue(target, info.GetValue(obj, null), null);
            }
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

