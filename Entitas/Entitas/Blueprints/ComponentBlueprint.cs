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

                // TODO
                throw new ComponentBlueprintException();
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

            // TODO
            throw new ComponentBlueprintException();
        }

        void cacheFieldInfos() {
            if (_fields != null) {
                _cachedFields = new Dictionary<string, FieldInfo>();
                foreach (var kv in _fields) {
                    var field = _type.GetField(kv.Key, BindingFlags.Instance | BindingFlags.Public);
                    if (field == null) {

                        // TODO
                        throw new ComponentBlueprintException();
                    }
                    _cachedFields.Add(kv.Key, field);
                }
            }
        }
    }

    public class ComponentBlueprintException : Exception {

    }
}