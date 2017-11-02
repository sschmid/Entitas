using System;
using Entitas.CodeGeneration.CodeGenerator;
using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public static class PluginUtil {

        public const string ASSEMBLY_RESOLVER_KEY = "Entitas.CodeGeneration.Plugins.AssemblyResolver";

        public static AssemblyResolver GetCachedAssemblyResolver(Dictionary<string, object> objectCache, string[] assemblies, string[] basePaths) {
            var key = ASSEMBLY_RESOLVER_KEY + "(" + assemblies.ToCSV() + ")";
            object cachedAssemblyResolver;
            if (!objectCache.TryGetValue(key , out cachedAssemblyResolver)) {
                cachedAssemblyResolver = new AssemblyResolver(AppDomain.CurrentDomain, basePaths);
                var resolver = (AssemblyResolver)cachedAssemblyResolver;
                foreach (var path in assemblies) {
                    resolver.Load(path);
                }
                objectCache.Add(key , cachedAssemblyResolver);
            }

            return (AssemblyResolver)cachedAssemblyResolver;
        }
    }
}
