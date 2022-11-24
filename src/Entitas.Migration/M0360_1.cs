using System.Linq;

namespace Entitas.Migration
{
    public class M0360_1 : IMigration
    {
        public string Version => "0.36.0-1";
        public string WorkingDirectory => "project root";
        public string Description => "Updates Entitas.properties to use renamed keys";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path, "Entitas.properties")
            .Select(file =>
            {
                file.FileContent = file.FileContent.Replace("Entitas.CodeGenerator.Pools", "Entitas.CodeGenerator.Contexts");
                return file;
            }).ToArray();
    }
}
