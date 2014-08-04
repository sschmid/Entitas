using Entitas;
using Entitas.CodeGenerator;




public class SomeComponent : IComponent {
}

public class SomeWithArgComponent : IComponent {
    public int arg;
}

public class SomeWithArgsComponent : IComponent {
    public int arg1;
    public string arg2;
}




[SingleEntity]
public class SingleComponent : IComponent {
}

[SingleEntity]
public class SingleWithArgComponent : IComponent {
    public int arg;
}

[SingleEntity]
public class SingleWithArgsComponent : IComponent {
    public int arg1;
    public string arg2;
}




public class SingletonComponent : IComponent {
    public static SingletonComponent singleton = new SingletonComponent();
}

[SingleEntity]
public class SingletonSingleEntityComponent : IComponent {
    public static SingletonComponent singleton = new SingletonComponent();
}


