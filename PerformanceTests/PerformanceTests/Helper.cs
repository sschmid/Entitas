using Entitas;
using Entitas.CodeGenerator;

public static class Helper {
    public static Context CreatePool() {
        return new Context(CP.NumComponents, 0, new ContextInfo(CodeGenerator.DEFAULT_POOL_NAME, new string[CP.NumComponents], null));
    }
}
