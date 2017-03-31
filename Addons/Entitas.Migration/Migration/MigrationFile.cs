namespace Entitas.Migration {

    public class MigrationFile {

        public string fileName;
        public string fileContent;

        public MigrationFile(string fileName, string fileContent) {
            this.fileName = fileName;
            this.fileContent = fileContent;
        }
    }
}
