using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class NamespaceDescription {
        public string name { get { return _name; } }
        public ClassDescription[] classDescriptions { get { return _classDescriptions.ToArray(); } }

        readonly string _name;
        readonly List<ClassDescription> _classDescriptions;

        public NamespaceDescription(string name) {
            _name = name;
            _classDescriptions = new List<ClassDescription>();
        }

        public ClassDescription AddClass(string name) {
            var classDescription = new ClassDescription(name);
            _classDescriptions.Add(classDescription);
            return classDescription;
        }
    }
}