using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public partial class CSharpFileBuilder {

        readonly new List<string> _usings;
        readonly List<NamespaceDescription> _namespaceDescriptions;

        public CSharpFileBuilder() {
            _usings = new List<string>();
            _namespaceDescriptions = new List<NamespaceDescription>();
        }

        public void AddUsing(string aUsing) {
            _usings.Add(aUsing);
        }

        public NamespaceDescription NoNamespace() {
            return AddNamespace(null);
        }

        public NamespaceDescription AddNamespace(string aNamespace) {
            var namespaceDescription = new NamespaceDescription(aNamespace);
            _namespaceDescriptions.Add(namespaceDescription);
            return namespaceDescription;
        }
    }
}
