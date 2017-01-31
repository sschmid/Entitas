using System;
using Entitas.CodeGenerator;
using Entitas.CodeGenerator.Api;

[VisualDebugging]
public class SomeClass {

    public string name;

    public SomeClass(string name) {
        this.name = name;
    }
}

[VisualDebugging]
public struct SomeStruct {

    public string name;

    public SomeStruct(string name) {
        this.name = name;
    }
}

namespace SomeNamespace {

    [VisualDebugging]
    public class SomeOtherClass {

        public string name;

        public SomeOtherClass(string name) {
            this.name = name;
        }
    }
}

[VisualDebugging]
public class SomeGenericClass<T> {
    public T value;
}

[VisualDebugging, Unique]
public class ISomeInterface {
}

[VisualDebugging, CustomComponentName("CoolNameComponent")]
public class BadName {
}
    
[Serializable, VisualDebugging, CustomComponentName("PositionComponent", "VelocityComponent")]
public struct IntVector2 {
    public int x;
    public int y;
}
