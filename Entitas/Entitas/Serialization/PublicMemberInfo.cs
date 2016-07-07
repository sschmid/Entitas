using System;
using System.Collections.Generic;
using System.Reflection;

namespace Entitas.Serialization {

    public class PublicMemberInfo {

        public Type type { get { return _type; } }
        public string name { get { return _name; } }

        readonly FieldInfo _fieldInfo;
        readonly PropertyInfo _propertyInfo;
        readonly Type _type;
        readonly string _name;

        public PublicMemberInfo(FieldInfo info) {
            _fieldInfo = info;
            _propertyInfo = null;
            _type = _fieldInfo.FieldType;
            _name = _fieldInfo.Name;
        }

        public PublicMemberInfo(PropertyInfo info) {
            _fieldInfo = null;
            _propertyInfo = info;
            _type = _propertyInfo.PropertyType;
            _name = _propertyInfo.Name;
        }

        public PublicMemberInfo(Type type, string name) {
            _type = type;
            _name = name;
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

        public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type) {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = type.GetFields(bindingFlags);
            var propertyInfos = type.GetProperties(bindingFlags);
            var memberInfos = new List<PublicMemberInfo>(fieldInfos.Length + propertyInfos.Length);

            for (int i = 0, fieldInfosLength = fieldInfos.Length; i < fieldInfosLength; i++) {
                memberInfos.Add(new PublicMemberInfo(fieldInfos[i]));
            }

            for (int i = 0, propertyInfosLength = propertyInfos.Length; i < propertyInfosLength; i++) {
                var info = propertyInfos[i];
                if (info.CanRead && info.CanWrite) {
                    memberInfos.Add(new PublicMemberInfo(info));
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
            for (int i = 0, memberInfosLength = memberInfos.Count; i < memberInfosLength; i++) {
                var info = memberInfos[i];
                info.SetValue(target, info.GetValue(source));
            }
        }
    }
}

