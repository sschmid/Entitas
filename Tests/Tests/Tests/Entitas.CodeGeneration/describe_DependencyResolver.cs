using System;
using Entitas.CodeGeneration.CodeGenerator;
using NSpec;

class describe_DependencyResolver : nspec {

    void when_resolving() {

        xcontext["when no dependencies"] = () => {

            string dllPath = null;
            Type[] types = null;

            before = () => {
                dllPath = TestExtensions.GetProjectRoot() + "/Tests/Tests.Fixtures/TestDependenyBase/bin/Release/TestDependenyBase.dll";
                var resolver = new DependencyResolver(AppDomain.CurrentDomain, new string[0]);
                resolver.Load(dllPath);
                types = resolver.GetTypes();
            };

            it["loads dll"] = () => {
                types.Length.should_be(1);
                types[0].FullName.should_be("TestDependenyBase.Point");
            };

            it["instantiates type"] = () => {
                Activator.CreateInstance(types[0]);
            };
        };

        xcontext["when dependencies"] = () => {

            string dllPath = null;
            Type[] types = null;

            before = () => {
                dllPath = TestExtensions.GetProjectRoot() + "/Tests/Tests.Fixtures/TestDependency/bin/Release/TestDependency.dll";
                var resolver = new DependencyResolver(AppDomain.CurrentDomain, new string[0]);
                resolver.Load(dllPath);
                types = resolver.GetTypes();
            };

            it["loads dll with all dependencies"] = () => {
                types.Length.should_be(1);
                types[0].FullName.should_be("TestDependency.PositionComponent");
            };

            it["instantiates type"] = () => {
                Activator.CreateInstance(types[0]);
            };
        };
    }
}
