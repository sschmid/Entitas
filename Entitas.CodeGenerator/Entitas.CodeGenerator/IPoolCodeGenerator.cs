#if UNITY_EDITOR
namespace Entitas.CodeGenerator {
    public interface IPoolCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(string[] poolNames);
    }
}
#endif
