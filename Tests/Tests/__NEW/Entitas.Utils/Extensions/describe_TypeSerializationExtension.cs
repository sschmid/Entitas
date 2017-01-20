using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Utils;
using NSpec;

class describe_TypeSerializationExtension : nspec {

    static string toCompilable<T>() {
        return typeof(T).ToCompilableString();
    }

    static Type toType(string typeString) {
        return typeString.ToType();
    }

    void when_generating() {

        context["when generating compilable string from type"] = () => {

            context["built-in types"] = () => {
                // https://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
                it["generates bool"] = () => toCompilable<bool>().should_be("bool");
                it["generates byte"] = () => toCompilable<byte>().should_be("byte");
                it["generates sbyte"] = () => toCompilable<sbyte>().should_be("sbyte");
                it["generates char"] = () => toCompilable<char>().should_be("char");
                it["generates decimal"] = () => toCompilable<decimal>().should_be("decimal");
                it["generates double"] = () => toCompilable<double>().should_be("double");
                it["generates float"] = () => toCompilable<float>().should_be("float");
                it["generates int"] = () => toCompilable<int>().should_be("int");
                it["generates uint"] = () => toCompilable<uint>().should_be("uint");
                it["generates long"] = () => toCompilable<long>().should_be("long");
                it["generates ulong"] = () => toCompilable<ulong>().should_be("ulong");
                it["generates object"] = () => toCompilable<object>().should_be("object");
                it["generates short"] = () => toCompilable<short>().should_be("short");
                it["generates ushort"] = () => toCompilable<ushort>().should_be("ushort");
                it["generates string"] = () => toCompilable<string>().should_be("string");
                it["generates void"] = () => typeof(void).ToCompilableString().should_be("void");
            };

            context["custom types"] = () => {
                it["generates type string with namespace"] = () => toCompilable<Entity>().should_be("Entitas.Entity");
            };

            context["array"] = () => {
                it["generates array rank 1"] = () => toCompilable<int[]>().should_be("int[]");
                it["generates array rank 2"] = () => toCompilable<int[,]>().should_be("int[,]");
                it["generates array rank 3"] = () => toCompilable<int[,,]>().should_be("int[,,]");
                it["generates array of arrays"] = () => toCompilable<int[][]>().should_be("int[][]");
            };

            context["generics"] = () => {
                it["generates List<T>"] = () => toCompilable<List<int>>().should_be("System.Collections.Generic.List<int>");
                it["generates HashSet<T>"] = () => toCompilable<HashSet<Entity>>().should_be("System.Collections.Generic.HashSet<Entitas.Entity>");
                it["generates Dictionary<T1, T2>"] = () => toCompilable<Dictionary<string, Entity>>().should_be("System.Collections.Generic.Dictionary<string, Entitas.Entity>");
            };

            context["enum"] = () => {
                it["generates enum"] = () => toCompilable<SomeEnum>().should_be("SomeEnum");
                it["generates nested enum"] = () => toCompilable<NestedTest.NestedTestEnum>().should_be("NestedTest.NestedTestEnum");
            };

            context["nested"] = () => {
                it["generates nested class"] = () => toCompilable<NestedClass.InnerClass>().should_be("NestedClass.InnerClass");
            };

            context["mixed"] = () => {
                it["generates List<T>[,]"] = () => toCompilable<List<int>[,]>().should_be("System.Collections.Generic.List<int>[,]");
                it["generates Dictionary<List<T>[,], T2>[]"] = () => toCompilable<Dictionary<List<NestedTest.NestedTestEnum>[,], Entity>[]>().should_be("System.Collections.Generic.Dictionary<System.Collections.Generic.List<NestedTest.NestedTestEnum>[,], Entitas.Entity>[]");
            };
        };

        context["when finding type from string"] = () => {

            context["built-in types"] = () => {
                it["finds bool"] = () => toType("bool").should_be(typeof(bool));
                it["finds byte"] = () => toType("byte").should_be(typeof(byte));
                it["finds sbyte"] = () => toType("sbyte").should_be(typeof(sbyte));
                it["finds char"] = () => toType("char").should_be(typeof(char));
                it["finds decimal"] = () => toType("decimal").should_be(typeof(decimal));
                it["finds double"] = () => toType("double").should_be(typeof(double));
                it["finds float"] = () => toType("float").should_be(typeof(float));
                it["finds int"] = () => toType("int").should_be(typeof(int));
                it["finds uint"] = () => toType("uint").should_be(typeof(uint));
                it["finds long"] = () => toType("long").should_be(typeof(long));
                it["finds ulong"] = () => toType("ulong").should_be(typeof(ulong));
                it["finds object"] = () => toType("object").should_be(typeof(object));
                it["finds short"] = () => toType("short").should_be(typeof(short));
                it["finds ushort"] = () => toType("ushort").should_be(typeof(ushort));
                it["finds string"] = () => toType("string").should_be(typeof(string));
                it["finds void"] = () => toType("void").should_be(typeof(void));
            };

            context["custom types"] = () => {
                it["finds type"] = () => toType("Entitas.Entity").should_be(typeof(Entity));
            };

            context["array"] = () => {
                it["finds array rank 1"] = () => toType("int[]").should_be(typeof(int[]));
                it["finds array rank 2"] = () => toType("int[,]").should_be(typeof(int[,]));
                it["finds array rank 3"] = () => toType("int[,,]").should_be(typeof(int[,,]));
                it["finds array of arrays"] = () => toType("int[][]").should_be(typeof(int[][]));
            };

            context["generics"] = () => {
                it["finds List<T>"] = () => toType("System.Collections.Generic.List<int>").should_be(typeof(List<int>));
                xit["finds HashSet<T>"] = () => toType("System.Collections.Generic.HashSet<Entitas.Entity>").should_be(typeof(HashSet<Entity>));
                xit["finds Dictionary<T1, T2>"] = () => toType("System.Collections.Generic.Dictionary<string, Entitas.Entity>").should_be(typeof(Dictionary<string, Entity>));
            };

            context["enum"] = () => {
                it["generates enum"] = () => toType("SomeEnum").should_be(typeof(SomeEnum));
                it["generates nested enum"] = () => toType("NestedTest+NestedTestEnum").should_be(typeof(NestedTest.NestedTestEnum));
            };

            context["nested"] = () => {
                it["generates nested class"] = () => toType("NestedClass+InnerClass").should_be(typeof(NestedClass.InnerClass));
            };

            context["mixed"] = () => {
                it["generates List<T>[,]"] = () => toType("System.Collections.Generic.List<int>[,]").should_be(typeof(List<int>[,]));
                xit["generates Dictionary<List<T>[,], T2>[]"] = () => toType("System.Collections.Generic.Dictionary<System.Collections.Generic.List<NestedTest+NestedTestEnum>[,], Entitas.Entity>[]").should_be(typeof(Dictionary<List<NestedTest.NestedTestEnum>[,], Entity>[]));
            };
        };
    }
}
