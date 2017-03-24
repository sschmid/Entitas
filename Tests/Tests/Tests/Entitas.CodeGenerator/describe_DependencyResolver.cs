using System;
using System.Linq;
using Entitas.CodeGenerator;
using NSpec;

class describe_DependencyResolver : nspec {

    void when_resolving() {

        xcontext["when no dependencies"] = () => {

            string dllPath = null;
            Type[] types = null;

            before = () => {
                dllPath = TestExtensions.GetProjectRoot() + "/Tests/Tests.Fixtures/TestDependenyBase/bin/Release/TestDependenyBase.dll";
                var resolver = new DependencyResolver(AppDomain.CurrentDomain);
                resolver.Load(dllPath);
                types = resolver.GetTypes();
            };

            it["loads dll"] = () => {
                types.should_contain(type => type.FullName == "TestDependenyBase.Point");
            };

            it["instantiates type"] = () => {
                var type = types.Single(t => t.FullName == "TestDependenyBase.Point");
                Activator.CreateInstance(type);
            };
        };

        context["when dependencies"] = () => {

            string dllPath = null;
            Type[] types = null;

            before = () => {
                dllPath = TestExtensions.GetProjectRoot() + "/Tests/Tests.Fixtures/TestDependency/bin/Release/TestDependency.dll";
                var resolver = new DependencyResolver(AppDomain.CurrentDomain);
                resolver.Load(dllPath);
                types = resolver.GetTypes();
            };

            it["loads dll with all dependencies"] = () => {
                types.should_contain(type => type.FullName == "TestDependency.PositionComponent");
            };

            it["instantiates type"] = () => {
                var type = types.Single(t => t.FullName == "TestDependency.PositionComponent");
                Activator.CreateInstance(type);
            };
        };
    }
}
