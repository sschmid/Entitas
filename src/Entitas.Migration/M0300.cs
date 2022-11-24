using System.Linq;

namespace Entitas.Migration
{
    public class M0300 : IMigration
    {
        public string Version => "0.30.0";
        public string WorkingDirectory => "project root";
        public string Description => "Updates Entitas.properties to use renamed code generators";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path, "Entitas.properties")
            .Select(file =>
            {
                file.FileContent = file.FileContent.Replace("ComponentsGenerator", "ComponentExtensionsGenerator");
                file.FileContent = file.FileContent.Replace("PoolAttributeGenerator", "PoolAttributesGenerator");
                return file;
            })
            .ToArray();
    }
}
