using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public class DependencyResolver {

        readonly AppDomain _appDomain;
        string _lastPath;

        public DependencyResolver(AppDomain appDomain) {
            _appDomain = appDomain;
            _appDomain.AssemblyResolve += onAssemblyResolve;
        }

        public void Load(string path) {
            _lastPath = path;
            _appDomain.Load(path);
        }

        Assembly onAssemblyResolve(object sender, ResolveEventArgs args) {

            Assembly assembly = null;
            try {
                assembly = Assembly.LoadFrom(args.Name);
            } catch(Exception) {
                var name = new AssemblyName(args.Name).Name;
                var path = Path.GetDirectoryName(_lastPath) + Path.DirectorySeparatorChar + name + ".dll";
                Assembly.LoadFrom(path);
            }

            return assembly;
        }

        public Type[] GetTypes() {
            return _appDomain.GetAssemblies()
                             .SelectMany(assembly => assembly.GetTypes())
                             .ToArray();
        }
    }
}
