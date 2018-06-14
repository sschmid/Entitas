public static class ContentExtend
{
    public static GameContext GameContex(this object obj)
    {
       return Contexts.sharedInstance.game;
    }

    public static Contexts Context(this object obj)
    {
        return Contexts.sharedInstance;
    }

    public static  InputContext GameInput(this object obj)
    {
        return Contexts.sharedInstance.input;
    }
}
