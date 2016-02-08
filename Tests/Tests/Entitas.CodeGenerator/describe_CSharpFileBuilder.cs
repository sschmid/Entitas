using System;
using Entitas.CodeGenerator;
using NSpec;
using Entitas;

class describe_CSharpFileBuilder : nspec {

    const bool logResults = false;

    void generates(CSharpFileBuilder cb, string expectedCode) {
        expectedCode = expectedCode.ToUnixLineEndings();
        var result = cb.ToString();
        #pragma warning disable
        if (logResults) {
            Console.WriteLine("should:\n'" + expectedCode + "'");
            Console.WriteLine("was:\n'" + result + "'");
        }
        result.should_be(expectedCode);
    }

    void when_building() {

        CSharpFileBuilder cb = null;
        before = () => {
            cb = new CSharpFileBuilder();
        };

        it["creates empty file"] = () => {
            generates(cb, string.Empty);
        };

        it["creates a namespace"] = () => {
            cb.AddNamespace("MyNamespace");
            generates(cb, @"namespace MyNamespace {
}");
        };

        it["creates multiple namespaces"] = () => {
            cb.AddNamespace("MyNamespace1");
            cb.AddNamespace("MyNamespace2");
            generates(cb, @"namespace MyNamespace1 {
}

namespace MyNamespace2 {
}");
        };

        it["ignores namespace when null or empty"] = () => {
            cb.AddNamespace(null);
            cb.AddNamespace(string.Empty);
            generates(cb, string.Empty);
        };

        it["adds usings"] = () => {
            cb.AddUsing("System");
            cb.AddUsing("Entitas");
            cb.AddNamespace("MyNamespace");
            generates(cb, @"using System;
using Entitas;

namespace MyNamespace {
}");
        };

        it["adds an empty class without namespace"] = () => {
            cb.NoNamespace().AddClass("MyClass");
            generates(cb, @"class MyClass {
}");
        };

        context["when added namespace"] = () => {

            NamespaceDescription aNamespace = null;
            before = () => {
                aNamespace = cb.AddNamespace("MyNamespace");
            };

            it["adds a class"] = () => {
                aNamespace.AddClass("MyClass");
                generates(cb, @"namespace MyNamespace {
    class MyClass {
    }
}");
            };

            it["adds multiple classes"] = () => {
                aNamespace.AddClass("MyClass1");
                aNamespace.AddClass("MyClass2");
                generates(cb, @"namespace MyNamespace {
    class MyClass1 {
    }

    class MyClass2 {
    }
}");
            };

            context["when added a class"] = () => {

                ClassDescription cd = null;
                before = () => {
                    cd = aNamespace.AddClass("MyClass");
                };

                it["adds modifiers"] = () => {
                    cd.AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Static);
                    generates(cb, @"namespace MyNamespace {
    public static class MyClass {
    }
}");
                };

                it["sets base class"] = () => {
                    cd.SetBaseClass(typeof(PoolAttribute).ToCompilableString());
                    generates(cb, @"namespace MyNamespace {
    class MyClass : Entitas.CodeGenerator.PoolAttribute {
    }
}");
                };

                it["adds an interface"] = () => {
                    cd.AddInterface(typeof(ISystem).ToCompilableString());
                    generates(cb, @"namespace MyNamespace {
    class MyClass : Entitas.ISystem {
    }
}");
                };

                it["adds multiple interfaces"] = () => {
                    cd.AddInterface(typeof(ISystem).ToCompilableString());
                    cd.AddInterface(typeof(IExecuteSystem).ToCompilableString());
                    generates(cb, @"namespace MyNamespace {
    class MyClass : Entitas.ISystem, Entitas.IExecuteSystem {
    }
}");
                };

                it["adds base class and interfaces"] = () => {
                    cd.SetBaseClass(typeof(PoolAttribute).ToCompilableString());
                    cd.AddInterface(typeof(ISystem).ToCompilableString());
                    cd.AddInterface(typeof(IExecuteSystem).ToCompilableString());
                    generates(cb, @"namespace MyNamespace {
    class MyClass : Entitas.CodeGenerator.PoolAttribute, Entitas.ISystem, Entitas.IExecuteSystem {
    }
}");
                };

                it["adds constructor"] = () => {
                    cd.AddConstructor();
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        MyClass() {
        }
    }
}");
                };

                it["adds multiple constructors"] = () => {
                    cd.AddConstructor();
                    cd.AddConstructor();
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        MyClass() {
        }

        MyClass() {
        }
    }
}");
                };

                it["adds a property"] = () => {
                    cd.AddProperty(typeof(string).ToCompilableString(), "name");
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        string name { get; set; }
    }
}");
                };

                it["adds constructor with body"] = () => {
                    cd.AddConstructor("var x = 42;\nvar y = 24;");
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        MyClass() {
            var x = 42;
            var y = 24;
        }
    }
}");
                };

                it["adds multiple properties"] = () => {
                    cd.AddProperty(typeof(string).ToCompilableString(), "name");
                    cd.AddProperty(typeof(int).ToCompilableString(), "age");
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        string name { get; set; }
        int age { get; set; }
    }
}");
                };

                it["adds a field of type"] = () => {
                    cd.AddField(typeof(string).ToCompilableString(), "_name");
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        string _name;
    }
}");
                };

                it["adds multiple fields of type"] = () => {
                    cd.AddField(typeof(string).ToCompilableString(), "_name");
                    cd.AddField(typeof(int).ToCompilableString(), "_age");
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        string _name;
        int _age;
    }
}");
                };

                it["adds a method"] = () => {
                    cd.AddMethod("MyMethod", @"var str = ""Hello"";
System.Console.WriteLine(str);");
                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        void MyMethod() {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                };

                it["adds multiple methods"] = () => {
                    cd.AddMethod("MyMethod1", @"var str = ""Hello"";
System.Console.WriteLine(str);");

                    cd.AddMethod("MyMethod2", @"var str = ""World"";
System.Console.WriteLine(str);");

                    generates(cb, @"namespace MyNamespace {
    class MyClass {
        void MyMethod1() {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }

        void MyMethod2() {
            var str = ""World"";
            System.Console.WriteLine(str);
        }
    }
}");
                };

                context["when constructor added"] = () => {

                    ConstructorDescription ctor = null;
                    before = () => {
                        ctor = cd.AddConstructor("var x = 42;\nvar y = 24;");
                    };

                    it["adds modifiers"] = () => {
                        ctor.AddModifier(AccessModifiers.Public);
                        ctor.AddModifier(Modifiers.Override);
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        public override MyClass() {
            var x = 42;
            var y = 24;
        }
    }
}");
                    };

                    it["adds a parameter"] = () => {
                        ctor.AddParameter(new MethodParameterDescription(typeof(string).ToCompilableString(), "name"));
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        MyClass(string name) {
            var x = 42;
            var y = 24;
        }
    }
}");
                    };

                    it["adds parameters"] = () => {
                        ctor.AddParameter(new MethodParameterDescription(typeof(string).ToCompilableString(), "name"))
                            .AddParameter(new MethodParameterDescription(typeof(int).ToCompilableString(), "age"));
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        MyClass(string name, int age) {
            var x = 42;
            var y = 24;
        }
    }
}");
                    };

                    it["calls base"] = () => {
                        ctor.CallBase("42, 24");
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        MyClass() : base(42, 24) {
            var x = 42;
            var y = 24;
        }
    }
}");
                    };
                };

                context["when property added"] = () => {

                    PropertyDescription pd = null;
                    before = () => {
                        pd = cd.AddProperty(typeof(int).ToCompilableString(), "age");
                    };

                    it["adds modifiers"] = () => {
                        pd.AddModifier(AccessModifiers.Public);
                        pd.AddModifier(Modifiers.Virtual);
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        public virtual int age { get; set; }
    }
}");
                    };

                    it["sets getter"] = () => {
                        pd.SetGetter("var a = 42;\nreturn a;");
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        int age {
            get {
                var a = 42;
                return a;
            }
        }
    }
}");
                    };

                    it["sets setter"] = () => {
                        pd.SetSetter("var newAge = value;\n_age = newAge;");
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        int age {
            set {
                var newAge = value;
                _age = newAge;
            }
        }
    }
}");
                    };

                    it["sets getter and setter"] = () => {
                        pd.SetGetter("var a = 42;\nreturn a;");
                        pd.SetSetter("var newAge = value;\n_age = newAge;");
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        int age {
            get {
                var a = 42;
                return a;
            }
            set {
                var newAge = value;
                _age = newAge;
            }
        }
    }
}");
                    };

                    it["adds a field"] = () => {
                        cd.AddField(typeof(string).ToCompilableString(), "_name");
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        int age { get; set; }

        string _name;
    }
}");
                    };

                    it["adds a method"] = () => {
                        cd.AddMethod("MyMethod", @"var str = ""Hello"";
System.Console.WriteLine(str);");
                        
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        int age { get; set; }

        void MyMethod() {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };
                };

                context["when field added"] = () => {

                    FieldDescription fd = null;
                    before = () => {
                        fd = cd.AddField(typeof(string).ToCompilableString(), "version");
                    };

                    it["adds modifiers"] = () => {
                        fd.AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Const);
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        public const string version;
    }
}");
                    };

                    it["sets default value"] = () => {
                        fd.SetValue("\"1.0.0\"");
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        string version = ""1.0.0"";
    }
}");
                    };

                    it["adds a method"] = () => {
                        cd.AddMethod("MyMethod", @"var str = ""Hello"";
System.Console.WriteLine(str);");

                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        string version;

        void MyMethod() {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };
                };

                context["when method added"] = () => {

                    MethodDescription md = null;
                    before = () => {
                        md = cd.AddMethod("MyMethod", "var str = \"Hello\";\nSystem.Console.WriteLine(str);");
                    };

                    it["adds modiefiers"] = () => {
                        md.AddModifier(AccessModifiers.Public).AddModifier(Modifiers.Override);
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        public override void MyMethod() {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };

                    it["sets returnType"] = () => {
                        md.SetReturnType(typeof(char[]).ToCompilableString());
                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        char[] MyMethod() {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };

                    it["adds parameters"] = () => {
                        md.AddParameter(new MethodParameterDescription(typeof(string).ToCompilableString(), "name"))
                            .AddParameter(new MethodParameterDescription(typeof(int).ToCompilableString(), "age"));

                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        void MyMethod(string name, int age) {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };

                    it["adds parameters with keyword"] = () => {
                        md.AddParameter(new MethodParameterDescription(typeof(string).ToCompilableString(), "name"))
                            .AddParameter(new MethodParameterDescription(typeof(int).ToCompilableString(), "age").SetKeyword(MethodParameterKeyword.Out));

                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        void MyMethod(string name, out int age) {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };

                    it["sets default value"] = () => {
                        md.AddParameter(new MethodParameterDescription(typeof(string).ToCompilableString(), "name"))
                            .AddParameter(new MethodParameterDescription(typeof(int).ToCompilableString(), "age").SetDefaultValue("42"));

                        generates(cb, @"namespace MyNamespace {
    class MyClass {
        void MyMethod(string name, int age = 42) {
            var str = ""Hello"";
            System.Console.WriteLine(str);
        }
    }
}");
                    };
                };
            };
        };
    }
}

