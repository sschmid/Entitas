using Entitas.CodeGeneration.CodeGenerator;
using System;

namespace Entitas.CodeGeneration.Plugins {

    public static class PluginUtil {

        // TODO Cache + unit tests
        public static DependencyResolver GetAssembliesResolver(string[] assemblies, string[] basePaths) {
            var resolver = new DependencyResolver(AppDomain.CurrentDomain, basePaths);
            foreach(var path in assemblies) {
                resolver.Load(path);
            }

            return resolver;
        }
    }
}
