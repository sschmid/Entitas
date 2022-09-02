namespace Entitas.Migration {

    public class M0360_1 : IMigration {

        public string version { get { return "0.36.0-1"; } }

        public string workingDirectory { get { return "project root"; } }

        public string description { get { return "Updates Entitas.properties to use renamed keys"; } }

        public MigrationFile[] Migrate(string path) {
            var properties = MigrationUtils.GetFiles(path, "Entitas.properties");

            for (int i = 0; i < properties.Length; i++) {
                var file = properties[i];

                // Entitas.CodeGenerator.Pools = Input,Core,Score

                file.fileContent = file.fileContent.Replace("Entitas.CodeGenerator.Pools", "Entitas.CodeGenerator.Contexts");
            }

            return properties;
        }
    }
}
