using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Entitas.Generators.Tests
{
    [UsesVerify]
    public class ContextGeneratorTests
    {
        static string FixturesPath => Path.Combine(TestExtensions.GetProjectRoot(),
            "tests", "Entitas.Generators.Tests.Fixtures");

        static Task Verify(string fixture) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, $"{fixture}.cs")), new ContextGenerator());

        static Task VerifyContext(string fixture) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, "Contexts", $"{fixture}.txt")), new ContextGenerator());

        [Fact]
        public Task SomeClass() => Verify("SomeClass");

        [Fact]
        public Task NamespacedContext() => VerifyContext("NamespacedContext");

        [Fact]
        public Task SomeContext() => VerifyContext("SomeContext");
    }
}
