using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration
{
    public class M0320 : IMigration
    {
        public string Version => "0.32.0";
        public string WorkingDirectory => "project root";
        public string Description => "Updates Entitas.properties to use renamed keys and updates calls to pool.CreateSystem<T>()";

        const string Pattern = @".CreateSystem<(?<system>\w*)>\(\s*\)";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path, "Entitas.properties")
            .Select(file =>
            {
                file.FileContent = file.FileContent.Replace("Entitas.Unity.CodeGenerator.GeneratedFolderPath", "Entitas.CodeGenerator.GeneratedFolderPath");
                file.FileContent = file.FileContent.Replace("Entitas.Unity.CodeGenerator.Pools", "Entitas.CodeGenerator.Pools");
                file.FileContent = file.FileContent.Replace("Entitas.Unity.CodeGenerator.EnabledCodeGenerators", "Entitas.CodeGenerator.EnabledCodeGenerators");
                return file;
            })
            .Concat(MigrationUtils.GetFiles(path)
                .Where(file => Regex.IsMatch(file.FileContent, Pattern))
                .Select(file =>
                {
                    file.FileContent = Regex.Replace(
                        file.FileContent,
                        Pattern,
                        match => $".CreateSystem(new {match.Groups["system"].Value}())"
                    );
                    return file;
                }))
            .ToArray();
    }
}
