namespace Entitas.Migration
{
    public class MigrationFile
    {
        public string FileName;
        public string FileContent;

        public MigrationFile(string fileName, string fileContent)
        {
            FileName = fileName;
            FileContent = fileContent;
        }
    }
}
