using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas.Serialization {

    public struct PublicMemberInfo {

        public Type type {
            get {
                return _fieldInfo != null
                    ? _fieldInfo.FieldType
                    : _propertyInfo.PropertyType;
            }
        }

        public string name {
            get {
                return _fieldInfo != null
                    ? _fieldInfo.Name
                    : _propertyInfo.Name;
            }
        }

        readonly FieldInfo _fieldInfo;
        readonly PropertyInfo _propertyInfo;

        public PublicMemberInfo(FieldInfo info) {
            _fieldInfo = info;
            _propertyInfo = null;
        }

        public PublicMemberInfo(PropertyInfo info) {
            _fieldInfo = null;
            _propertyInfo = info;
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
    }

    public static class PublicMemberInfoExtension {

        public static PublicMemberInfo[] GetPublicMemberInfos(this Type type) {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var fieldInfos = type.GetFields(bindingFlags).Select(info => new PublicMemberInfo(info));
            var propertyInfos = type.GetProperties(bindingFlags)
                .Where(info => info.CanRead && info.CanWrite)
                .Select(info => new PublicMemberInfo(info));

            return fieldInfos.Concat(propertyInfos).ToArray();
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
            for (int i = 0, memberInfosLength = memberInfos.Length; i < memberInfosLength; i++) {
                var info = memberInfos[i];
                info.SetValue(target, info.GetValue(source));
            }
        }
    }
}

