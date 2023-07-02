using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MyFeature;
using VerifyXunit;

namespace Entitas.Generators.Tests
{
    public static class TestHelper
    {
        // https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/
        public static Task Verify(string source, IIncrementalGenerator generator)
        {
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .Concat(new[]
                {
                    MetadataReference.CreateFromFile(typeof(IComponent).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Attributes.ContextAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(SomeNamespacedComponent).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(MyApp.LibraryContext).Assembly.Location)
                });

            var compilation = CSharpCompilation.Create(
                "Entitas.Generators.Tests",
                new[] { CSharpSyntaxTree.ParseText(source) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var driver = CSharpGeneratorDriver.Create(generator).RunGenerators(compilation);
            return Verifier.Verify(driver).UseDirectory("snapshots");
        }

        public static void AssertUsesGlobalNamespaces(string code)
        {
            var patterns = new[]
            {
                "System",
                "Entitas"
            }.Select(word => $"(?<!\\busing )(?<!\\bstatic )(?<!\\bnamespace )(?<!\\bglobal::)(?<!\"){word}");

            foreach (var pattern in patterns)
            {
                Regex.Matches(code, pattern).Should().HaveCount(0);
            }
        }
    }
}
