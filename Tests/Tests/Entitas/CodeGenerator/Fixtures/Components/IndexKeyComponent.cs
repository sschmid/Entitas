using Entitas;
using Entitas.CodeGenerator;

public class IndexKeyComponent : IComponent {

    [IndexKey("MyName")]
    public string name;
}

[Pool]
public class DefautPoolIndexKeyComponent : IComponent {

    [IndexKey("MyName")]
    public string name;
}

[Pool("Meta")]
public class MetaIndexKeyComponent : IComponent {

    [IndexKey("MyName")]
    public string name;
}
