using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0450Tests
    {
        static string FixturePath => Path.Combine(TestExtensions.GetProjectRoot(), "tests", "Entitas.Migration.Tests", "fixtures", "exclude", "M0450");

        readonly M0450 _migration;

        public M0450Tests()
        {
            _migration = new M0450();
        }

        [Fact]
        public void FindsAllFiles()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(1);
            updatedFiles.Any(file => file.FileName == Path.Combine(FixturePath, "Entitas.properties")).Should().BeTrue();
        }

        [Fact]
        public void UpdatesEntitasProperties()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var file = updatedFiles[0];

            file.FileContent.Contains("Entitas.CodeGeneration.CodeGenerator.SearchPaths").Should().BeFalse();
            file.FileContent.Contains("CodeGenerator.SearchPaths").Should().BeTrue();

            file.FileContent.Contains("Entitas.CodeGeneration.CodeGenerator.Plugins").Should().BeFalse();
            file.FileContent.Contains("CodeGenerator.Plugins").Should().BeTrue();

            file.FileContent.Contains("Entitas.CodeGeneration.CodeGenerator.DataProviders").Should().BeFalse();
            file.FileContent.Contains("CodeGenerator.DataProviders").Should().BeTrue();

            file.FileContent.Contains("Entitas.CodeGeneration.CodeGenerator.CodeGenerators").Should().BeFalse();
            file.FileContent.Contains("CodeGenerator.CodeGenerators").Should().BeTrue();

            file.FileContent.Contains("Entitas.CodeGeneration.CodeGenerator.PostProcessors").Should().BeFalse();
            file.FileContent.Contains("CodeGenerator.PostProcessors").Should().BeTrue();

            file.FileContent.Contains("Entitas.CodeGeneration.CodeGenerator.CLI.Ignore.UnusedKeys").Should().BeFalse();
            file.FileContent.Contains("CodeGenerator.CLI.Ignore.UnusedKeys").Should().BeTrue();
        }
    }
}
