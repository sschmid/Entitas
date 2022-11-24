using System.Linq;

namespace Entitas.Migration
{
    public class M0260 : IMigration
    {
        public string Version => "0.26.0";
        public string WorkingDirectory => "where generated files are located";
        public string Description => "Deactivates code to prevent compile erros";

        const string PoolPattern1 = @"var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(";
        const string PoolPattern2 = @"UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);";

        const string ComponentPattern = @"throw new SingleEntityException(";

        const string Replacement = @"//";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path)
            .Where(file => file.FileContent.Contains(PoolPattern1) || file.FileContent.Contains(ComponentPattern))
            .Select(file =>
            {
                file.FileContent = file.FileContent.Replace(PoolPattern1, Replacement + PoolPattern1);
                file.FileContent = file.FileContent.Replace(PoolPattern2, Replacement + PoolPattern2);
                file.FileContent = file.FileContent.Replace(ComponentPattern, Replacement + ComponentPattern);
                return file;
            })
            .ToArray();
    }
}
