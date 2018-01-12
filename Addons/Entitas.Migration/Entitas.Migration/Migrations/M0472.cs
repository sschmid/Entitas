namespace Entitas.Migration {

    public class M0472 : IMigration {

        public string version { get { return "0.47.2"; } }

        public string workingDirectory { get { return "project root"; } }

        public string description { get { return "Updates properties to use renamed keys"; } }

        public MigrationFile[] Migrate(string path) {
            var properties = MigrationUtils.GetFiles(path, "*.properties");

            for (int i = 0; i < properties.Length; i++) {
                var file = properties[i];

                file.fileContent = file.fileContent.Replace("CodeGenerator.SearchPaths", "Jenny.SearchPaths");

                file.fileContent = file.fileContent.Replace("CodeGenerator.Plugins", "Jenny.Plugins");

                file.fileContent = file.fileContent.Replace("CodeGenerator.PreProcessors", "Jenny.PreProcessors");
                file.fileContent = file.fileContent.Replace("CodeGenerator.DataProviders", "Jenny.DataProviders");
                file.fileContent = file.fileContent.Replace("CodeGenerator.CodeGenerators", "Jenny.CodeGenerators");
                file.fileContent = file.fileContent.Replace("CodeGenerator.PostProcessors", "Jenny.PostProcessors");

                file.fileContent = file.fileContent.Replace("CodeGenerator.CLI.Ignore.UnusedKeys", "Jenny.Ignore.Keys");
                file.fileContent = file.fileContent.Replace("Ignore.Keys", "Jenny.Ignore.Keys");

                file.fileContent = file.fileContent.Replace("CodeGenerator.Server.Port", "Jenny.Server.Port");
                file.fileContent = file.fileContent.Replace("CodeGenerator.Client.Host", "Jenny.Client.Host");
            }

            return properties;
        }
    }
}
