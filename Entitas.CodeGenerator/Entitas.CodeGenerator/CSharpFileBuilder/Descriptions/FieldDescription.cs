using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class FieldDescription {
        public string type { get { return _type; } }
        public string name { get { return _name; } }
        public string[] modifiers { get { return _modifiers.ToArray(); } }
        public string value { get { return _value; } }

        readonly string _type;
        readonly string _name;
        readonly List<string> _modifiers;
        string _value;

        public FieldDescription(string type, string name) {
            _type = type;
            _name = name;
            _modifiers = new List<string>();
        }

        public FieldDescription AddModifier(string modifier) {
            _modifiers.Add(modifier);
            return this;
        }

        public FieldDescription SetValue(string value) {
            _value = value;
            return this;
        }
    }
}