using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration {

    public class M0320 : IMigration {

        public string version { get { return "0.32.0"; } }

        public string workingDirectory { get { return "project root"; } }

        public string description { get { return "Updates Entitas.properties to use renamed keys and updates calls to pool.CreateSystem<T>()"; } }

        public MigrationFile[] Migrate(string path) {
            var properties = MigrationUtils.GetFiles(path, "Entitas.properties");

            for (int i = 0; i < properties.Length; i++) {
                var file = properties[i];

                //Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath = Assets/Editor/DefaultInstanceCreator/
                //Entitas.Unity.VisualDebugging.TypeDrawerFolderPath = Assets/Editor/TypeDrawer/

                file.fileContent = file.fileContent.Replace("Entitas.Unity.CodeGenerator.GeneratedFolderPath", "Entitas.CodeGenerator.GeneratedFolderPath");
                file.fileContent = file.fileContent.Replace("Entitas.Unity.CodeGenerator.Pools", "Entitas.CodeGenerator.Pools");
                file.fileContent = file.fileContent.Replace("Entitas.Unity.CodeGenerator.EnabledCodeGenerators", "Entitas.CodeGenerator.EnabledCodeGenerators");
            }

            const string pattern = @".CreateSystem<(?<system>\w*)>\(\s*\)";

            var sources = MigrationUtils.GetFiles(path)
                .Where(file => Regex.IsMatch(file.fileContent, pattern))
                .ToArray();

            for (int i = 0; i < sources.Length; i++) {
                var file = sources[i];

                file.fileContent = Regex.Replace(
                    file.fileContent,
                    pattern,
                    match => ".CreateSystem(new " + match.Groups["system"].Value + "())"
                );
            }

            return properties.Concat(sources).ToArray();
        }
    }
}
