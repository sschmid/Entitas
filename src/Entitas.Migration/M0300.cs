namespace Entitas.Migration
{
    public class M0300 : IMigration
    {
        public string version => "0.30.0";
        public string workingDirectory => "project root";
        public string description => "Updates Entitas.properties to use renamed code generators";

        public MigrationFile[] Migrate(string path)
        {
            var files = MigrationUtils.GetFiles(path, "Entitas.properties");
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                file.fileContent = file.fileContent.Replace("ComponentsGenerator", "ComponentExtensionsGenerator");
                file.fileContent = file.fileContent.Replace("PoolAttributeGenerator", "PoolAttributesGenerator");
            }

            return files;
        }
    }
}
