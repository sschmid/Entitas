using System.Collections.Generic;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public abstract class AbstractGenerator : ICodeGenerator, IConfigurable {

        public abstract string name { get; }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
            CodeGeneratorExtentions.ignoreNamespaces = _ignoreNamespacesConfig.ignoreNamespaces;
        }

        public abstract CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
