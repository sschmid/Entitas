namespace MyApp
{
    public static partial class ContextInitialization
    {
        [MyApp.Main.ContextInitialization]
        public static partial void InitializeMain();

        [Other.ContextInitializationAttribute]
        public static partial void InitializeOther();
    }
}
