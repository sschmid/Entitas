using System;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextsPCLGenerator : ContextsGenerator {
        public static string CONTEXT_USING = "using System.Reflection;";
        public static string CONTEXT_CONSTRUCTORS_PCL_LIST_TEMPLATE = @"        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetRuntimeMethods(),
            method => method.GetCustomAttribute(typeof(Entitas.CodeGeneration.Attributes.PostConstructorAttribute))!=null
        );";
        public override bool isEnabledByDefault {
            get {
                return false;
            }
        }
        public override string generateContextConstructorsList() {
            return CONTEXT_CONSTRUCTORS_PCL_LIST_TEMPLATE;
        }
        public override string[] getUsingDeclarations() {
            return new string[1] { CONTEXT_USING };
        }
    }
}