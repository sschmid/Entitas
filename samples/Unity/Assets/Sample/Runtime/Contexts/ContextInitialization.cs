using Entitas.Generators.Attributes;

public static partial class ContextInitialization
{
    public static void InitializeAllContexts()
    {
        InitializeGameContext();
        InitializeInputContext();
    }

    [ContextInitialization(typeof(GameContext))]
    public static partial void InitializeGameContext();

    [ContextInitialization(typeof(InputContext))]
    public static partial void InitializeInputContext();
}
