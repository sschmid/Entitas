using System;

namespace Entitas.CodeGenerator {
    public interface ISystemCodeGenerator : ICodeGenerator {
        CodeGenFile[] Generate(Type[] systems);
    }
}

