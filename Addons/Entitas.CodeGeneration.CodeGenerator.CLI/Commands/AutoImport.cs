using System.IO;
using System.Linq;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class AutoImport : AbstractCommand {

        public override string trigger { get { return "auto-import"; } }
        public override string description { get { return "Find and import all plugins"; } }
        public override string example { get { return "entitas auto-import"; } }

        protected override void run() {
            fabl.Debug(_preferences.ToString());
            autoImport();
            new FixConfig().Run(_rawArgs);
        }

        void autoImport() {
            var config = new CodeGeneratorConfig();
            config.Configure(_preferences);

            var plugins = config.searchPaths
                .SelectMany(path => Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                .Where(path => path.ToLower().EndsWith(".plugins.dll"));

            config.searchPaths = config.searchPaths
                .Concat(plugins.Select(path => Path.GetDirectoryName(path)))
                .Distinct()
                .ToArray();

            config.plugins = plugins
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .Distinct()
                .ToArray();

            _preferences.Save();
        }
    }
}
