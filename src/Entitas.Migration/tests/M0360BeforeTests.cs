using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0360BeforeTests
    {
        static string FixturePath => $"{TestExtensions.GetProjectRoot()}/src/Entitas.Migration/fixtures/M0360";

        readonly M0360_1 _migration;

        public M0360BeforeTests()
        {
            _migration = new M0360_1();
        }

        [Fact]
        public void FindsAllFiles()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            updatedFiles.Length.Should().Be(1);
            updatedFiles.Any(file => file.fileName == Path.Combine(FixturePath, "Entitas.properties")).Should().BeTrue();
        }

        [Fact]
        public void UpdatesEntitasProperties()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var file = updatedFiles[0];
            file.fileContent.Contains("Entitas.CodeGenerator.Pools").Should().BeFalse();
            file.fileContent.Contains("Entitas.CodeGenerator.Contexts").Should().BeTrue();
        }
    }
}
