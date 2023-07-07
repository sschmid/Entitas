using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
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

            var driver = CSharpGeneratorDriver
                .Create(generator)
                .WithUpdatedAnalyzerConfigOptions(new TestAnalyzerConfigOptionsProvider(new Dictionary<string, string>
                {
                    { EntitasAnalyzerConfigOptions.TestValueKey, "true" }
                }))
                .RunGenerators(compilation);

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

    sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; }

        public TestAnalyzerConfigOptionsProvider(Dictionary<string, string> options)
        {
            GlobalOptions = new DictionaryAnalyzerConfigOptions(options.ToImmutableDictionary());
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => DictionaryAnalyzerConfigOptions.Empty;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => DictionaryAnalyzerConfigOptions.Empty;
    }

    sealed class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        static readonly ImmutableDictionary<string, string> EmptyDictionary = ImmutableDictionary.Create<string, string>(KeyComparer);

        public static DictionaryAnalyzerConfigOptions Empty { get; } = new DictionaryAnalyzerConfigOptions(EmptyDictionary);

        readonly ImmutableDictionary<string, string> _options;

        public DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string> options) => _options = options;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _options.TryGetValue(key, out value);
    }
}
