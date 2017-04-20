using System.Collections.Generic;
using System.Linq;

namespace Entitas.Migration {

    public class M0410_3 : IMigration {

        public string version { get { return "0.41.0-3"; } }

        public string workingDirectory { get { return "where custom TypeDrawers are located"; } }

        public string description { get { return "Updating namespaces"; } }

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetFiles(path);

            var migratedFiles = new List<MigrationFile>();

            migratedFiles.AddRange(updateNamespace(files, "Entitas.Unity.VisualDebugging", "Entitas.VisualDebugging.Unity.Editor"));

            return migratedFiles.ToArray();
        }

        MigrationFile[] updateNamespace(MigrationFile[] files, string oldNamespace, string newNamespace) {
            var filesToMigrate = files.Where(f => f.fileContent.Contains(oldNamespace)).ToArray();
            foreach (var file in filesToMigrate) {
                file.fileContent = file.fileContent.Replace(oldNamespace, newNamespace);
            }

            return filesToMigrate;
        }
    }
}
