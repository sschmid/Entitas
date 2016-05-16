using Entitas;
using Entitas.CodeGenerator;

public class IndexKeyComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(IndexKeyComponent) })[0];
        }
    }

    [IndexKey("MyName")]
    public string name;
}

[Pool]
public class DefautPoolIndexKeyComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(DefautPoolIndexKeyComponent) })[0];
        }
    }

    [IndexKey("MyName")]
    public string name;
}

[Pool("Meta")]
public class MetaIndexKeyComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(MetaIndexKeyComponent) })[0];
        }
    }

    [IndexKey("MyName")]
    public string name;
}
