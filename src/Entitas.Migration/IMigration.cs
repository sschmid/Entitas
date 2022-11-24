namespace Entitas.Migration
{
    public interface IMigration
    {
        string Version { get; }
        string WorkingDirectory { get; }
        string Description { get; }

        MigrationFile[] Migrate(string path);
    }
}
