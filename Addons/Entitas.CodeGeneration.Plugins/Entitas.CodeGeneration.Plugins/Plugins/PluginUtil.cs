using System;
using Entitas.CodeGeneration.CodeGenerator;
using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public static class PluginUtil {

        static Dictionary<string, DependencyResolver> _resolvers = new Dictionary<string, DependencyResolver>();

        public static DependencyResolver GetAssembliesResolver(string[] assemblies, string[] basePaths) {
            var key = assemblies.ToCSV();
            if(!_resolvers.ContainsKey(key)) {
                var resolver = new DependencyResolver(AppDomain.CurrentDomain, basePaths);
                foreach(var path in assemblies) {
                    resolver.Load(path);
                }
                _resolvers.Add(key, resolver);
            }

            return _resolvers[key];
        }
    }
}
