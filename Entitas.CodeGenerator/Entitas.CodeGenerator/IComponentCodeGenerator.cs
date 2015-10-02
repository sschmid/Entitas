#if UNITY_EDITOR
using System;

namespace Entitas.CodeGenerator {
    public interface IComponentCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(Type[] components);
    }
}
#endif
