using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class MethodDescription {
        public string name { get { return _name; } }
        public string body { get { return _body; } }
        public string[] modifiers { get { return _modifiers.ToArray(); } }
        public string returnType { get { return _returnType; } }
        public MethodParameterDescription[] parameters { get { return _parameters.ToArray(); } }

        readonly string _name;
        readonly string _body;
        readonly List<string> _modifiers;
        string _returnType;
        readonly List<MethodParameterDescription> _parameters;

        public MethodDescription(string name, string body) {
            _name = name;
            _body = body;
            _modifiers = new List<string>();
            _parameters = new List<MethodParameterDescription>();
        }

        public MethodDescription AddModifier(string modifier) {
            _modifiers.Add(modifier);
            return this;
        }

        public MethodDescription SetReturnType(string type) {
            _returnType = type;
            return this;
        }

        public MethodDescription AddParameter(MethodParameterDescription parameter) {
            _parameters.Add(parameter);
            return this;
        }
    }
}