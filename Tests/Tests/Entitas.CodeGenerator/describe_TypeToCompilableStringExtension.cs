using NSpec;
using Entitas.CodeGenerator;
using System.Collections.Generic;
using Entitas;

class describe_TypeToCompilableStringExtension : nspec {

    static string generate<T>() {
        return typeof(T).ToCompilableString();
    }

    void when_generating() {

        context["built-in types"] = () => {
            // https://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
            it["generates bool"] = () => generate<bool>().should_be("bool");
            it["generates byte"] = () => generate<byte>().should_be("byte");
            it["generates sbyte"] = () => generate<sbyte>().should_be("sbyte");
            it["generates char"] = () => generate<char>().should_be("char");
            it["generates decimal"] = () => generate<decimal>().should_be("decimal");
            it["generates double"] = () => generate<double>().should_be("double");
            it["generates float"] = () => generate<float>().should_be("float");
            it["generates int"] = () => generate<int>().should_be("int");
            it["generates uint"] = () => generate<uint>().should_be("uint");
            it["generates long"] = () => generate<long>().should_be("long");
            it["generates ulong"] = () => generate<ulong>().should_be("ulong");
            it["generates object"] = () => generate<object>().should_be("object");
            it["generates short"] = () => generate<short>().should_be("short");
            it["generates ushort"] = () => generate<ushort>().should_be("ushort");
            it["generates string"] = () => generate<string>().should_be("string");
            it["generates void"] = () => typeof(void).ToCompilableString().should_be("void");
        };

        context["custom types"] = () => {
            it["generates type string with namespace"] = () => generate<Entity>().should_be("Entitas.Entity");
        };

        context["array"] = () => {
            it["generates array rank 1"] = () => generate<int[]>().should_be("int[]");
            it["generates array rank 2"] = () => generate<int[,]>().should_be("int[,]");
            it["generates array rank 3"] = () => generate<int[,,]>().should_be("int[,,]");
            it["generates array of arrays"] = () => generate<int[][]>().should_be("int[][]");
        };

        context["generics"] = () => {
            it["generates List<T>"] = () => generate<List<int>>().should_be("System.Collections.Generic.List<int>");
            it["generates HashSet<T>"] = () => generate<HashSet<Entity>>().should_be("System.Collections.Generic.HashSet<Entitas.Entity>");
            it["generates Dictionary<T1, T2>"] = () => generate<Dictionary<string, Entity>>().should_be("System.Collections.Generic.Dictionary<string, Entitas.Entity>");
        };

        context["enum"] = () => {
            it["generates enum"] = () => generate<TestEnum>().should_be("TestEnum");
            it["generates nested enum"] = () => generate<NestedTest.NestedTestEnum>().should_be("NestedTest.NestedTestEnum");
        };

        context["nested"] = () => {
            it["generates nested class"] = () => generate<NestedClass.InnerClass>().should_be("NestedClass.InnerClass");
        };

        context["mixed"] = () => {
            it["generates List<T>[,]"] = () => generate<List<int>[,]>().should_be("System.Collections.Generic.List<int>[,]");
            it["generates Dictionary<List<T>[,], T2>[]"] = () => generate<Dictionary<List<NestedTest.NestedTestEnum>[,], Entity>[]>().should_be("System.Collections.Generic.Dictionary<System.Collections.Generic.List<NestedTest.NestedTestEnum>[,], Entitas.Entity>[]");
        };
    }
}

