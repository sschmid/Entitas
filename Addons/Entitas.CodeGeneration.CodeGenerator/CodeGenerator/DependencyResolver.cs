using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fabl;

namespace Entitas.CodeGenerator {

    public class DependencyResolver {

        static Logger _logger = fabl.GetLogger(typeof(DependencyResolver).Name);

        readonly AppDomain _appDomain;
        string[] _basePaths;
        List<Assembly> _assemblies;

        public DependencyResolver(AppDomain appDomain, string[] basePaths) {
            _appDomain = appDomain;
            _appDomain.AssemblyResolve += onAssemblyResolve;
            _basePaths = basePaths;
            _assemblies = new List<Assembly>();
        }

        public void Load(string path) {
            _logger.Debug("AppDomain load: " + path);
            var assembly = _appDomain.Load(path);
            _assemblies.Add(assembly);
        }

        Assembly onAssemblyResolve(object sender, ResolveEventArgs args) {
            Assembly assembly = null;
            try {
                _logger.Debug("  - Loading: " + args.Name);
                assembly = Assembly.LoadFrom(args.Name);
            } catch(Exception) {
                var name = new AssemblyName(args.Name).Name;
                if(!name.EndsWith(".dll", StringComparison.Ordinal)) {
                    name += ".dll";
                }

                var path = resolvePath(name);
                if(path != null) {
                    assembly = Assembly.LoadFrom(path);
                }
            }

            return assembly;
        }

        string resolvePath(string assemblyName) {
            foreach(var basePath in _basePaths) {
                var path = basePath + Path.DirectorySeparatorChar + assemblyName;
                if(File.Exists(path)) {
                    _logger.Debug("    - Resolved: " + path);
                    return path;
                }
            }

            _logger.Warn("    - Could not resolve: " + assemblyName);
            return null;
        }

        public Type[] GetTypes() {
            var types = new List<Type>();
            foreach(var assembly in _assemblies) {
                try {
                    types.AddRange(assembly.GetTypes());
                } catch(ReflectionTypeLoadException ex) {
                    types.AddRange(ex.Types.Where(type => type != null));
                }
            }

            return types.ToArray();
        }
    }
}
