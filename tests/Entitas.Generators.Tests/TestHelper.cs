using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        public static readonly string ProjectRoot = GetProjectRoot();

        static string GetProjectRoot()
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current!.Name != "Entitas" && current.Name != "Entitas-CSharp") current = current.Parent;
            return current.FullName;
        }

        // https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/
        public static Task Verify(string source, IIncrementalGenerator generator, Dictionary<string, string> options)
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
                .WithUpdatedAnalyzerConfigOptions(new TestAnalyzerConfigOptionsProvider(options))
                .RunGenerators(compilation);

            return Verifier.Verify(driver).UseDirectory("snapshots");
        }

        public static void AssertUsesGlobalNamespaces(string path)
        {
            var code = File.ReadAllText(Path.Combine(ProjectRoot, path));

            var ignores = new[]
            {
                "global::Entitas.EntitasException",
                "global::Entitas.Systems",
                "EntitasAnalyzerConfigOptions"
            };

            foreach (var ignore in ignores)
            {
                code = code.Replace(ignore, string.Empty);
            }

            var patterns = new[]
            {
                "System",
                "Entitas"
            }.Select(word => $@"(?<!\busing )" +
                             $@"(?<!\bstatic )" +
                             $@"(?<!\bnamespace )" +
                             $@"(?<!\bglobal::)" +
                             $@"(?<!"")" +
                             $@"(?<!\w)" +
                             $"{word}");

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(code, pattern);
                matches.Should().HaveCount(0, $"because {path} should not use {pattern}\n{string.Join("\n", matches.Select(match => match.Value))}");
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

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;
    }

    sealed class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        readonly ImmutableDictionary<string, string> _options;
        public DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string> options) => _options = options;
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _options.TryGetValue(key, out value);
    }
}
