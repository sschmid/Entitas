using Entitas;

public class ComponentA : IComponent {}
public class ComponentB : IComponent {}
public class ComponentC : IComponent {}
public class ComponentD : IComponent {}
public class ComponentE : IComponent {}
public class ComponentF : IComponent {}

public static class Component {
    public static readonly ComponentA A = new ComponentA();
    public static readonly ComponentB B = new ComponentB();
    public static readonly ComponentC C = new ComponentC();
    public static readonly ComponentD D = new ComponentD();
    public static readonly ComponentE E = new ComponentE();
    public static readonly ComponentF F = new ComponentF();
}

public static class CID {
    public const int None = 0;
    public const int ComponentA = 1;
    public const int ComponentB = 2;
    public const int ComponentC = 3;
    public const int ComponentD = 4;
    public const int ComponentE = 5;
    public const int ComponentF = 6;

    public const int NumComponents = 7;
}

