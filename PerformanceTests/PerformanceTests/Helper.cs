using Entitas;
using Entitas.Api;

public static class Helper {

    public static IContext<Entity> CreateContext() {
        return new Context<Entity>(CP.NumComponents, 0, new ContextInfo("Test Context", new string[CP.NumComponents], null));
    }
}
