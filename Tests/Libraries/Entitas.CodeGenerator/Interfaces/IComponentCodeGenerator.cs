namespace Entitas.CodeGenerator {
    public interface IComponentCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(ComponentInfo[] componentInfos);
    }
}

