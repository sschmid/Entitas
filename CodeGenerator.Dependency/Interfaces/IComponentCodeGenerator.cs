namespace Entitas.CodeGeneration {
    public interface IComponentCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(ComponentInfo[] componentInfos);
    }
}

