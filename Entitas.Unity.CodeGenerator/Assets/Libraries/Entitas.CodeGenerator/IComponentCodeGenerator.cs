using System;

namespace Entitas.CodeGenerator {
    public interface IComponentCodeGenerator {
        CodeGenFile[] Generate(Type[] components);
    }
}

