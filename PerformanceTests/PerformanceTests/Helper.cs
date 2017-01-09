using Entitas;
using Entitas.CodeGenerator;

public static class Helper {
    public static Context CreateContext() {
        return new Context(CP.NumComponents, 0, new ContextInfo(CodeGenerator.DEFAULT_CONTEXT_NAME, new string[CP.NumComponents], null));
    }
}
