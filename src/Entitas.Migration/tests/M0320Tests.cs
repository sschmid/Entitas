using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0320Tests
    {
        static string FixturePath => $"{TestExtensions.GetProjectRoot()}/src/Entitas.Migration/fixtures/M0320";

        readonly M0320 _migration;

        public M0320Tests()
        {
            _migration = new M0320();
        }

        [Fact]
        public void FindsAllFiles()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(2);
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturePath, "Entitas.properties")).Should().BeTrue();
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturePath, "Systems.cs")).Should().BeTrue();
        }

        [Fact]
        public void UpdatesEntitasProperties()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var file = updatedFiles[0];

            file.fileContent.Contains("Entitas.Unity.CodeGenerator.GeneratedFolderPath").Should().BeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.GeneratedFolderPath").Should().BeTrue();

            file.fileContent.Contains("Entitas.Unity.CodeGenerator.Pools").Should().BeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.Pools").Should().BeTrue();

            file.fileContent.Contains("Entitas.Unity.CodeGenerator.EnabledCodeGenerators").Should().BeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.EnabledCodeGenerators").Should().BeTrue();

            // Ignores Entitas.Unity.VisualDebugging
            file.fileContent.Contains("Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath").Should().BeTrue();
            file.fileContent.Contains("Entitas.Unity.VisualDebugging.TypeDrawerFolderPath").Should().BeTrue();
        }

        [Fact]
        public void UpdatesPoolCreateSystem()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var file = updatedFiles[1];
            file.fileContent.Should().Be("pool.CreateSystem(new MySystem1());\npool.CreateSystem(new MySystem2());\n");
        }
    }
}
