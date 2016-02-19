using System;
using System.Collections.Generic;
using System.Reflection;
using Entitas;

namespace Entitas {

    public class ComponentBlueprint {

        public int index { get { return _index; } }
        public string fullTypeName { get { return _fullTypeName; } }
        public Dictionary<string, object> fields { get { return _fields; } }
        public Type type { get { return _type; } }

        readonly int _index;
        readonly string _fullTypeName;
        readonly Dictionary<string, object> _fields;
        readonly Type _type;

        Dictionary<string, FieldInfo> _cachedFields;

        public ComponentBlueprint(int index, string fullTypeName, Dictionary<string, object> fields) {
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

        public IComponent CreateComponent() {
            var component = (IComponent)Activator.CreateInstance(_type);
            if (_fields != null) {
                foreach (var kv in _fields) {
                    _cachedFields[kv.Key].SetValue(component, kv.Value);
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
                foreach (var kv in _fields) {
                    var field = _type.GetField(kv.Key, BindingFlags.Instance | BindingFlags.Public);
                    if (field == null) {
                        throw new ComponentBlueprintException("Could not find field '" + kv.Key + "' in Type '" + _type.FullName + "'!",
                            "Only non-static public fields are supported.");
                    }
                    _cachedFields.Add(kv.Key, field);
                }
            }
        }
    }

    public class ComponentBlueprintException : EntitasException {
        public ComponentBlueprintException(string message, string hint) : base(message, hint) {
        }
    }
}