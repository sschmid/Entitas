using Entitas;
using Entitas.CodeGenerator;

public static class Helper {
    public static IContext<XXXEntity> CreateContext() {
        return new XXXContext<XXXEntity>(CP.NumComponents, 0, new ContextInfo(CodeGenerator.DEFAULT_CONTEXT_NAME, new string[CP.NumComponents], null));
    }
}
