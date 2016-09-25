using System;
using System.Collections.Generic;
using System.Reflection;

namespace Entitas.Serialization {

    public class PublicMemberInfo {

        public readonly Type type;
        public readonly string name;
        public readonly AttributeInfo[] attributes;

        readonly FieldInfo _fieldInfo;
        readonly PropertyInfo _propertyInfo;

        public PublicMemberInfo(FieldInfo info) {
            _fieldInfo = info;
            _propertyInfo = null;
            type = _fieldInfo.FieldType;
            name = _fieldInfo.Name;
            attributes = getAttributes(_fieldInfo.GetCustomAttributes(false));
        }

        public PublicMemberInfo(PropertyInfo info) {
            _fieldInfo = null;
            _propertyInfo = info;
            type = _propertyInfo.PropertyType;
            name = _propertyInfo.Name;
            attributes = getAttributes(_propertyInfo.GetCustomAttributes(false));
        }

        public PublicMemberInfo(Type type, string name, AttributeInfo[] attributes = null) {
            this.type = type;
            this.name = name;
            this.attributes = attributes;
        }

        public object GetValue(object obj) {
            return _fieldInfo != null
                ? _fieldInfo.GetValue(obj)
                : _propertyInfo.GetValue(obj, null);
        }

        public void SetValue(object obj, object value) {
            if (_fieldInfo != null) {
                _fieldInfo.SetValue(obj, value);
            } else {
                _propertyInfo.SetValue(obj, value, null);                
            }
        }

        static AttributeInfo[] getAttributes(object[] attributes) {
            var infos = new AttributeInfo[attributes.Length];
            for (int i = 0; i < attributes.Length; i++) {
                var attr = attributes[i];
                infos[i] = new AttributeInfo(attr, attr.GetType().GetPublicMemberInfos());
            }

            return infos;
        }
    }

    public class AttributeInfo {

        public readonly object attribute;
        public readonly List<PublicMemberInfo> memberInfos;

        public AttributeInfo(object attribute, List<PublicMemberInfo> memberInfos) {
            this.attribute = attribute;
            this.memberInfos = memberInfos;
        }
    }

    public static class PublicMemberInfoExtension {

        public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type) {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = type.GetFields(bindingFlags);
            var propertyInfos = type.GetProperties(bindingFlags);
            var memberInfos = new List<PublicMemberInfo>(fieldInfos.Length + propertyInfos.Length);

            for (int i = 0; i < fieldInfos.Length; i++) {
                memberInfos.Add(new PublicMemberInfo(fieldInfos[i]));
            }

            for (int i = 0; i < propertyInfos.Length; i++) {
                var propertyInfo = propertyInfos[i];
                if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                    memberInfos.Add(new PublicMemberInfo(propertyInfo));
                }
            }

            return memberInfos;
        }

        public static object PublicMemberClone(this object obj) {
            var clone = Activator.CreateInstance(obj.GetType());
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static T PublicMemberClone<T>(this object obj) where T : new() {
            var clone = new T();
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static void CopyPublicMemberValues(this object source, object target) {
            var memberInfos = source.GetType().GetPublicMemberInfos();
            for (int i = 0; i < memberInfos.Count; i++) {
                var info = memberInfos[i];
                info.SetValue(target, info.GetValue(source));
            }
        }
    }
}

