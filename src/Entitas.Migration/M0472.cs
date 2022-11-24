namespace Entitas.Migration
{
    public class M0472 : IMigration
    {
        public string version => "0.47.2";
        public string workingDirectory => "project root";
        public string description => "Updates properties to use renamed keys";

        public MigrationFile[] Migrate(string path)
        {
            var properties = MigrationUtils.GetFiles(path, "*.properties");

            for (var i = 0; i < properties.Length; i++)
            {
                var file = properties[i];

                file.FileContent = file.FileContent.Replace("CodeGenerator.SearchPaths", "Jenny.SearchPaths");

                file.FileContent = file.FileContent.Replace("CodeGenerator.Plugins", "Jenny.Plugins");

                file.FileContent = file.FileContent.Replace("CodeGenerator.PreProcessors", "Jenny.PreProcessors");
                file.FileContent = file.FileContent.Replace("CodeGenerator.DataProviders", "Jenny.DataProviders");
                file.FileContent = file.FileContent.Replace("CodeGenerator.CodeGenerators", "Jenny.CodeGenerators");
                file.FileContent = file.FileContent.Replace("CodeGenerator.PostProcessors", "Jenny.PostProcessors");

                file.FileContent = file.FileContent.Replace("CodeGenerator.CLI.Ignore.UnusedKeys", "Jenny.Ignore.Keys");
                file.FileContent = file.FileContent.Replace("Ignore.Keys", "Jenny.Ignore.Keys");

                file.FileContent = file.FileContent.Replace("CodeGenerator.Server.Port", "Jenny.Server.Port");
                file.FileContent = file.FileContent.Replace("CodeGenerator.Client.Host", "Jenny.Client.Host");
            }

            return properties;
        }
    }
}
