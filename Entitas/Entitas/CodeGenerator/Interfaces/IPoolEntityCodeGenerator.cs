namespace Entitas.CodeGenerator {
    public interface IPoolEntityCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(ComponentInfo[] componentInfos);
    }
}
