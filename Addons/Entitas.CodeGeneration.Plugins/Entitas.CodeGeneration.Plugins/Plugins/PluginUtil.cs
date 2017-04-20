using System;
using Entitas.CodeGeneration.CodeGenerator;
using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public static class PluginUtil {

        static Dictionary<string, AssemblyResolver> _resolvers = new Dictionary<string, AssemblyResolver>();

        public static AssemblyResolver GetAssembliesResolver(string[] assemblies, string[] basePaths) {
            var key = assemblies.ToCSV();
            if (!_resolvers.ContainsKey(key)) {
                var resolver = new AssemblyResolver(AppDomain.CurrentDomain, basePaths);
                foreach (var path in assemblies) {
                    resolver.Load(path);
                }
                _resolvers.Add(key, resolver);
            }

            return _resolvers[key];
        }
    }
}
