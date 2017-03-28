using System;
using System.IO;
using System.Reflection;
using Fabl;

namespace Entitas.CodeGenerator {

    public class DependencyResolver {

        static Logger _logger = fabl.GetLogger(typeof(DependencyResolver).Name);

        readonly AppDomain _appDomain;
        string[] _basePaths;

        public DependencyResolver(AppDomain appDomain, string[] basePaths) {
            _appDomain = appDomain;
            _appDomain.AssemblyResolve += onAssemblyResolve;
            _basePaths = basePaths;
        }

        public void Load(string path) {
            _logger.Debug("- AppDomain add: " + path);
            _appDomain.Load(path);
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
            return TypeUtils.GetAllTypes();
        }
    }
}
