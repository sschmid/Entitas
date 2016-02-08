using System;
using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class PropertyDescription {
        public Type type { get { return _type; } }
        public string name { get { return _name; } }
        public string[] modifiers { get { return _modifiers.ToArray(); } }
        public string getter { get { return _getter; } }
        public string setter { get { return _setter; } }

        readonly Type _type;
        readonly string _name;
        readonly List<string> _modifiers;
        string _getter;
        string _setter;

        public PropertyDescription(Type type, string name) {
            _type = type;
            _name = name;
            _modifiers = new List<string>();
        }

        public PropertyDescription AddModifier(string modifier) {
            _modifiers.Add(modifier);
            return this;
        }

        public PropertyDescription SetGetter(string body) {
            _getter = body;
            return this;
        }

        public PropertyDescription SetSetter(string body) {
            _setter = body;
            return this;
        }
    }
}