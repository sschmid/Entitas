using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0300Tests
    {
        static string FixturePath => $"{TestExtensions.GetProjectRoot()}/src/Entitas.Migration/fixtures/M0300";

        readonly M0300 _migration;

        public M0300Tests()
        {
            _migration = new M0300();
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
            file.fileContent.Contains("ComponentsGenerator").Should().BeFalse();
            file.fileContent.Contains("ComponentExtensionsGenerator").Should().BeTrue();
            file.fileContent.Contains("PoolAttributeGenerator").Should().BeFalse();
            file.fileContent.Contains("PoolAttributesGenerator").Should().BeTrue();
        }
    }
}
