using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public abstract class AbstractGenerator : ICodeGenerator {

        public abstract string name { get; }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public abstract CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
