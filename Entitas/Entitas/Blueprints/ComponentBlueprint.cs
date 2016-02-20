using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Entitas;

namespace Entitas {

    [Serializable]
    public class SerializableField {
        public string fieldName;
        public object value;
    }

    [Serializable]
    public class ComponentBlueprint : ISerializable {

        public int index { get { return _index; } }
        public string fullTypeName { get { return _fullTypeName; } }
        public SerializableField[] fields { get { return _fields; } }
        public Type type { get { return _type; } }

        readonly int _index;
        readonly string _fullTypeName;
        readonly SerializableField[] _fields;

        const string FIELD_INDEX = "_index";
        const string FIELD_FULLTYPENAME = "_fullTypeName";
        const string FIELD_FIELDS = "_fields";

        readonly Type _type;

        Dictionary<string, FieldInfo> _cachedFields;

        public ComponentBlueprint(int index, string fullTypeName, params SerializableField[] fields) {
            _index = index;
            _fullTypeName = fullTypeName;
            _fields = fields;

            _type = getComponentType(fullTypeName);

            if (!_type.ImplementsInterface<IComponent>()) {
                throw new ComponentBlueprintException("Type '" + _type.FullName + "' doesn't implement IComponent!",
                    "The specified Type has to implement IComponent in order to create a " + typeof(ComponentBlueprint).Name + "" + ".");
            }

            cacheFieldInfos();
        }

        ComponentBlueprint(SerializationInfo info, StreamingContext context)
            : this(
                info.GetInt32(FIELD_INDEX),
                info.GetString(FIELD_FULLTYPENAME),
                (SerializableField[])info.GetValue(FIELD_FIELDS, typeof(SerializableField[]))
            ) {
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue(FIELD_INDEX, _index);
            info.AddValue(FIELD_FULLTYPENAME, _fullTypeName);
            info.AddValue(FIELD_FIELDS, _fields);
        }

        public IComponent CreateComponent() {
            var component = (IComponent)Activator.CreateInstance(_type);
            if (_fields != null) {
                for (int i = 0, fieldsLength = _fields.Length; i < fieldsLength; i++) {
                    var field = _fields[i];
                    _cachedFields[field.fieldName].SetValue(component, field.value);
                }
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

        void cacheFieldInfos() {
            if (_fields != null) {
                _cachedFields = new Dictionary<string, FieldInfo>();
                for (int i = 0, fieldsLength = _fields.Length; i < fieldsLength; i++) {
                    var field = _fields[i];
                    var fieldInfo = _type.GetField(field.fieldName, BindingFlags.Instance | BindingFlags.Public);
                    if (fieldInfo == null) {
                        throw new ComponentBlueprintException("Could not find field '" + field.fieldName + "' in Type '" + _type.FullName + "'!", "Only non-static public fields are supported.");
                    }
                    _cachedFields.Add(field.fieldName, fieldInfo);
                }
            }
        }
    }

    public class ComponentBlueprintException : EntitasException {
        public ComponentBlueprintException(string message, string hint) : base(message, hint) {
        }
    }
}