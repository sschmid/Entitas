using Entitas;

[Game]
public class MyFlagsComponent : IComponent
{
    [System.Flags]
    public enum MyFlags
    {
        Item1 = 1,
        Item2 = 2,
        Item3 = 4,
        Item4 = 8
    }

    public MyFlags Value;
}
