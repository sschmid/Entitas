using System.Linq;

namespace Entitas.Migration {

    public class M0260 : IMigration {

        public string version { get { return "0.26.0"; } }

        public string workingDirectory { get { return "where generated files are located"; } }

        public string description { get { return "Deactivates code to prevent compile erros"; } }

        const string POOL_PATTERN_1 = @"var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(";
        const string POOL_PATTERN_2 = @"UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);";

        const string COMPONENT_PATTERN = @"throw new SingleEntityException(";

        const string REPLACEMENT = @"//";

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetFiles(path)
                .Where(file => file.fileContent.Contains(POOL_PATTERN_1) || file.fileContent.Contains(COMPONENT_PATTERN))
                .ToArray();

            for (int i = 0; i < files.Length; i++) {
                var file = files[i];
                file.fileContent = file.fileContent.Replace(POOL_PATTERN_1, REPLACEMENT + POOL_PATTERN_1);
                file.fileContent = file.fileContent.Replace(POOL_PATTERN_2, REPLACEMENT + POOL_PATTERN_2);
                file.fileContent = file.fileContent.Replace(COMPONENT_PATTERN, REPLACEMENT + COMPONENT_PATTERN);
            }

            return files;
        }
    }
}
