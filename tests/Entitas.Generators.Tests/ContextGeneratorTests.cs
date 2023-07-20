using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using static Entitas.Generators.Tests.TestHelper;

namespace Entitas.Generators.Tests
{
    [UsesVerify]
    public class ContextGeneratorTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();

        static readonly string FixturesPath = Path.Combine(ProjectRoot, "tests", "Entitas.Generators.Tests", "fixtures");

        static Task Verify(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, $"{fixture}.cs")), new ContextGenerator(), options);

        static Task VerifyContext(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, "Contexts", $"{fixture}.txt")), new ContextGenerator(), options);

        static readonly Dictionary<string, string> DefaultOptions = new Dictionary<string, string>();

        [Theory]
        [InlineData("ContextGenerator")]
        [InlineData("ContextGenerator.ComponentIndex")]
        [InlineData("ContextGenerator.Context")]
        [InlineData("ContextGenerator.Entity")]
        [InlineData("ContextGenerator.Matcher")]
        public void UsesGlobalNamespace(string path)
        {
            AssertUsesGlobalNamespaces(Path.Combine("gen", "Entitas.Generators", "Context", path + ".cs"));
        }

        [Fact]
        public Task SomeClass() => Verify("SomeClass", DefaultOptions);

        [Fact]
        public Task NamespacedContext() => VerifyContext("NamespacedContext", DefaultOptions);

        [Fact]
        public Task SomeContext() => VerifyContext("SomeContext", DefaultOptions);
    }
}
