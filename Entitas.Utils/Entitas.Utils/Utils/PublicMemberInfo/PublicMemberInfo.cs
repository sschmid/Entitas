using System;
using System.Reflection;

namespace Entitas.Utils {

    public class PublicMemberInfo {

        public readonly Type type;
        public readonly string name;
        public readonly AttributeInfo[] attributes;

        readonly FieldInfo _fieldInfo;
        readonly PropertyInfo _propertyInfo;

        public PublicMemberInfo(FieldInfo info) {
            _fieldInfo = info;
            type = _fieldInfo.FieldType;
            name = _fieldInfo.Name;
            attributes = getAttributes(_fieldInfo.GetCustomAttributes(false));
        }

        public PublicMemberInfo(PropertyInfo info) {
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
}
