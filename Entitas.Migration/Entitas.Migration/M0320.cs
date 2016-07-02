namespace Entitas.Migration {

    public class M0320 : IMigration {

        public string version { get { return "0.32.0"; } }

        public string workingDirectory { get { return "project root"; } }

        public string description { get { return "Updates Entitas.properties to use renamed keys"; } }

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetFiles(path, "Entitas.properties");

            for (int i = 0; i < files.Length; i++) {
                var file = files[i];


                //Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath = Assets/Editor/DefaultInstanceCreator/
                //    Entitas.Unity.VisualDebugging.TypeDrawerFolderPath = Assets/Editor/TypeDrawer/

                file.fileContent = file.fileContent.Replace("Entitas.Unity.CodeGenerator.GeneratedFolderPath", "Entitas.CodeGenerator.GeneratedFolderPath");
                file.fileContent = file.fileContent.Replace("Entitas.Unity.CodeGenerator.Pools", "Entitas.CodeGenerator.Pools");
                file.fileContent = file.fileContent.Replace("Entitas.Unity.CodeGenerator.EnabledCodeGenerators", "Entitas.CodeGenerator.EnabledCodeGenerators");

                files[i] = file;
            }

            return files;
        }
    }
}

