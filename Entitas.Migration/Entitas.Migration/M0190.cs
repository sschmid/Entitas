using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration {

    public class M0190 : IMigration {

        public string version { get { return "0.19.0"; } }

        public string workingDirectory { get { return "where all systems are located"; } }

        public string description { get { return "Migrates IReactiveSystem.Execute to accept List<Entity>"; } }

        const string EXECUTE_PATTERN = @"public\s*void\s*Execute\s*\(\s*Entity\s*\[\s*\]\s*entities\s*\)";
        const string EXECUTE_REPLACEMENT = "public void Execute(System.Collections.Generic.List<Entity> entities)";

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetFiles(path)
                .Where(file => Regex.IsMatch(file.fileContent, EXECUTE_PATTERN))
                .ToArray();

            for (int i = 0; i < files.Length; i++) {
                files[i].fileContent = Regex.Replace(files[i].fileContent, EXECUTE_PATTERN, EXECUTE_REPLACEMENT, RegexOptions.Multiline);
            }

            return files;
        }
    }
}
