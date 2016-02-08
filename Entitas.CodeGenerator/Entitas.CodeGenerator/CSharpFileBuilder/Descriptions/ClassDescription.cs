using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class ClassDescription {
        public string name { get { return _name; } }
        public string[] modifiers { get { return _modifiers.ToArray(); } }
        public string baseClass { get { return _baseClass; } }
        public List<string> interfaces { get { return _interfaces; } }
        public ConstructorDescription[] constructorDescriptions { get { return _constructorDescriptions.ToArray(); } }
        public PropertyDescription[] propertyDescriptions { get { return _propertyDescriptions.ToArray(); } }
        public FieldDescription[] fieldDescriptions { get { return _fieldDescriptions.ToArray(); } }
        public MethodDescription[] methodDescriptions { get { return _methodDescriptions.ToArray(); } }

        readonly string _name;
        readonly List<string> _modifiers;
        string _baseClass;
        readonly List<string> _interfaces;
        readonly List<ConstructorDescription> _constructorDescriptions;
        readonly List<PropertyDescription> _propertyDescriptions;
        readonly List<FieldDescription> _fieldDescriptions;
        readonly List<MethodDescription> _methodDescriptions;

        public ClassDescription(string name) {
            _name = name;
            _modifiers = new List<string>();
            _interfaces = new List<string>();
            _constructorDescriptions = new List<ConstructorDescription>();
            _propertyDescriptions = new List<PropertyDescription>();
            _fieldDescriptions = new List<FieldDescription>();
            _methodDescriptions = new List<MethodDescription>();
        }

        public ClassDescription AddModifier(string modifier) {
            _modifiers.Add(modifier);
            return this;
        }

        public ClassDescription SetBaseClass(string type) {
            _baseClass = type;
            return this;
        }

        public ClassDescription AddInterface(string type) {
            _interfaces.Add(type);
            return this;
        }

        public ConstructorDescription AddConstructor(string body = "") {
            var constructorDescription = new ConstructorDescription(_name, body);
            _constructorDescriptions.Add(constructorDescription);
            return constructorDescription;
        }

        public PropertyDescription AddProperty(string type, string name) {
            var propertyDescription = new PropertyDescription(type, name);
            _propertyDescriptions.Add(propertyDescription);
            return propertyDescription;
        }

        public ClassDescription AddBackingField(PropertyDescription pd) {
            AddField(pd.type, "_" + pd.name);
            return this;
        }

        public FieldDescription AddField(string type, string name) {
            var fieldDescription = new FieldDescription(type, name);
            _fieldDescriptions.Add(fieldDescription);
            return fieldDescription;
        }

        public MethodDescription AddMethod(string name, string body) {
            var methodDescription = new MethodDescription(name, body);
            _methodDescriptions.Add(methodDescription);
            return methodDescription;
        }
    }
}