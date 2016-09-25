namespace Entitas.Migration {

    public interface IMigration {

        string version { get; }
        string workingDirectory { get; }
        string description { get; }

        MigrationFile[] Migrate(string path);
    }
}
