using System.Linq;

namespace Entitas.Migration
{
    public class M0450 : IMigration
    {
        public string Version => "0.45.0";
        public string WorkingDirectory => "project root";
        public string Description => "Updates Entitas.properties to use renamed keys";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path, "Entitas.properties")
            .Select(file =>
            {
                file.FileContent = file.FileContent.Replace("Entitas.CodeGeneration.CodeGenerator.SearchPaths", "CodeGenerator.SearchPaths");
                file.FileContent = file.FileContent.Replace("Entitas.CodeGeneration.CodeGenerator.Plugins", "CodeGenerator.Plugins");
                file.FileContent = file.FileContent.Replace("Entitas.CodeGeneration.CodeGenerator.DataProviders", "CodeGenerator.DataProviders");
                file.FileContent = file.FileContent.Replace("Entitas.CodeGeneration.CodeGenerator.CodeGenerators", "CodeGenerator.CodeGenerators");
                file.FileContent = file.FileContent.Replace("Entitas.CodeGeneration.CodeGenerator.PostProcessors", "CodeGenerator.PostProcessors");
                file.FileContent = file.FileContent.Replace("Entitas.CodeGeneration.CodeGenerator.CLI.Ignore.UnusedKeys", "CodeGenerator.CLI.Ignore.UnusedKeys");
                return file;
            })
            .ToArray();
    }
}
