using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using Entitas.CodeGenerator;

[VisualDebugging]
public class SomeClass {
    public string name;

    public SomeClass(string name) {
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

[VisualDebugging]
public class ISomeInterface {
}

[VisualDebugging, CustomComponentName("CoolNameComponent")]
public class BadName {
}
    