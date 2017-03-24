using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public class DependencyResolver {

        readonly AppDomain _appDomain;
        string[] _basePaths;

        public DependencyResolver(AppDomain appDomain, string[] basePaths) {
            _appDomain = appDomain;
            _appDomain.AssemblyResolve += onAssemblyResolve;
            _basePaths = basePaths;
        }

        public void Load(string path) {
            //Console.WriteLine("- AppDomain add: " + path);
            _appDomain.Load(path);
        }

        Assembly onAssemblyResolve(object sender, ResolveEventArgs args) {
            Assembly assembly = null;
            try {
                //Console.WriteLine("  - Loading: " + args.Name);
                assembly = Assembly.LoadFrom(args.Name);
            } catch(Exception) {
                var name = new AssemblyName(args.Name).Name;
                if(!name.EndsWith(".dll", StringComparison.Ordinal)) {
                    name += ".dll";
                }

                foreach(var basePath in _basePaths) {
                    var path = basePath + Path.DirectorySeparatorChar + name;
                    if(File.Exists(path)) {
                        //Console.WriteLine("    - Resolved: " + path);
                        assembly = Assembly.LoadFrom(path);
                    }
                }
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
