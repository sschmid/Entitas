using System.Collections.Generic;
using DesperateDevs.Reflection;

namespace Entitas.Plugins
{
    public static class PluginUtil
    {
        public const string AssemblyResolverKey = "Entitas.CodeGeneration.Plugins.AssemblyResolver";

        public static AssemblyResolver GetCachedAssemblyResolver(Dictionary<string, object> objectCache, string[] assemblies, string[] basePaths)
        {
            if (!objectCache.TryGetValue(AssemblyResolverKey, out var cachedAssemblyResolver))
            {
                cachedAssemblyResolver = new AssemblyResolver(basePaths);
                var resolver = (AssemblyResolver)cachedAssemblyResolver;
                foreach (var path in assemblies)
                    resolver.Load(path);

                objectCache.Add(AssemblyResolverKey, cachedAssemblyResolver);
            }

            return (AssemblyResolver)cachedAssemblyResolver;
        }
    }
}
