using Entitas;

public class ComponentA : IComponent {
}

public class ComponentB : IComponent {
}

public class ComponentC : IComponent {
}

public static class Component {
    public static readonly ComponentA A = new ComponentA();
    public static readonly ComponentB B = new ComponentB();
    public static readonly ComponentC C = new ComponentC();
}

public static class CID {
    public const int None = 0;
    public const int ComponentA = 1;
    public const int ComponentB = 2;
    public const int ComponentC = 3;

    public const int NumComponents = 4;
}

