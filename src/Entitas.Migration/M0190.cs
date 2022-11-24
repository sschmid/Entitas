using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration
{
    public class M0190 : IMigration
    {
        public string Version => "0.19.0";
        public string WorkingDirectory => "where all systems are located";
        public string Description => "Migrates IReactiveSystem.Execute to accept List<Entity>";

        const string ExecutePattern = @"public\s*void\s*Execute\s*\(\s*Entity\s*\[\s*\]\s*entities\s*\)";
        const string ExecuteReplacement = "public void Execute(System.Collections.Generic.List<Entity> entities)";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path)
            .Where(file => Regex.IsMatch(file.FileContent, ExecutePattern))
            .Select(file =>
            {
                file.FileContent = Regex.Replace(file.FileContent, ExecutePattern, ExecuteReplacement, RegexOptions.Multiline);
                return file;
            })
            .ToArray();
    }
}
