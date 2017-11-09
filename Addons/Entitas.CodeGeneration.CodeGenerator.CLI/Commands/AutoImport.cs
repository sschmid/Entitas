using System.IO;
using System.Linq;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class AutoImport : AbstractCommand {

        public override string trigger { get { return "auto-import"; } }
        public override string description { get { return "Find and import all plugins"; } }
        public override string example { get { return "entitas auto-import"; } }

        public override void Run(string[] args) {
            if (assertPreferences(args)) {
                var preferences = loadPreferences(args);
                fabl.Debug(preferences.ToString());
                autoImport(preferences);
                new FixConfig().Run(args);
            }
        }

        static void autoImport(Preferences preferences) {
            var config = new CodeGeneratorConfig();
            config.Configure(preferences);

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

            preferences.Save();
        }
    }
}
