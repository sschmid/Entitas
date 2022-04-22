using System.Collections.Generic;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public abstract class AbstractGenerator : ICodeGenerator, IConfigurable {

        public abstract string Name { get; }
        public int Order { get { return 0; } }
        public bool RunInDryMode { get { return true; } }

        public Dictionary<string, string> DefaultProperties { get { return _ignoreNamespacesConfig.DefaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
            CodeGeneratorExtensions.ignoreNamespaces = _ignoreNamespacesConfig.ignoreNamespaces;
        }

        public abstract CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
