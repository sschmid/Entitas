using System;

namespace Entitas.CodeGenerator {
    public interface ISystemCodeGenerator {
        CodeGenFile[] Generate(Type[] systems);
    }
}

