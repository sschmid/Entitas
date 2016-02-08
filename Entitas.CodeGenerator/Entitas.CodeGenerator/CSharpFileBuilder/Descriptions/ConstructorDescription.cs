using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class ConstructorDescription {
        public string name { get { return _name; } }
        public string body { get { return _body; } }
        public string[] modifiers { get { return _modifiers.ToArray(); } }
        public MethodParameterDescription[] parameters { get { return _parameters.ToArray(); } }
        public string baseCall { get { return _baseCall; } }

        readonly string _name;
        readonly string _body;
        readonly List<string> _modifiers;
        readonly List<MethodParameterDescription> _parameters;
        string _baseCall;

        public ConstructorDescription(string name, string body) {
            _name = name;
            _body = body;
            _modifiers = new List<string>();
            _parameters = new List<MethodParameterDescription>();
        }

        public ConstructorDescription AddModifier(string modifier) {
            _modifiers.Add(modifier);
            return this;
        }

        public ConstructorDescription AddParameter(MethodParameterDescription parameter) {
            _parameters.Add(parameter);
            return this;
        }

        public void CallBase(string arguments) {
            _baseCall = arguments;
        }
    }
}