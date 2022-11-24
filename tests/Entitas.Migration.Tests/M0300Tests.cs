using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Migration.Tests
{
    public class M0300Tests
    {
        static string FixturePath => Path.Combine(TestExtensions.GetProjectRoot(), "tests", "Entitas.Migration.Tests", "fixtures", "exclude", "M0300");

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
            updatedFiles.Any(file => file.FileName == Path.Combine(FixturePath, "Entitas.properties")).Should().BeTrue();
        }

        [Fact]
        public void UpdatesEntitasProperties()
        {
            var updatedFiles = _migration.Migrate(FixturePath);
            var file = updatedFiles[0];
            file.FileContent.Contains("ComponentsGenerator").Should().BeFalse();
            file.FileContent.Contains("ComponentExtensionsGenerator").Should().BeTrue();
            file.FileContent.Contains("PoolAttributeGenerator").Should().BeFalse();
            file.FileContent.Contains("PoolAttributesGenerator").Should().BeTrue();
        }
    }
}
