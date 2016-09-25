using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NSpec;

class check_namespaces : nspec {

    static string dir(params string[] paths) {
        return paths.Aggregate(string.Empty, (pathString, p) => pathString + p + Path.DirectorySeparatorChar);
    }

    void when_checking_namespaces() {
        var projectRoot = TestExtensions.GetProjectRoot();
        var sourceFiles = TestExtensions.GetSourceFiles(projectRoot);

        it["processes roughly the correct number of files"] = () => {
            sourceFiles.Count.should_be_greater_than(80);
            sourceFiles.Count.should_be_less_than(150);
        };

        const string namespacePattern = @"(?:^namespace)\s.*\b";
        string expectedNamespacePattern = string.Format(@"[^\{0}]*", Path.DirectorySeparatorChar);

        var entitasSourceDir = dir("Entitas", "Entitas");
        var entitasUnitySourceDir = dir("Entitas.Unity", "Assets", "Entitas", "Unity");

        var each = new Each<string, string, string>();

        foreach(var file in sourceFiles) {

            string expectedNamespace;

            var fileName = file.Key
                .Replace(dir(projectRoot), string.Empty)
                .Replace(entitasSourceDir + "CodeGenerator", "Entitas.CodeGenerator")
                .Replace(entitasSourceDir + dir("Serialization", "Blueprints"), "Entitas.Serialization.Blueprints/")
                .Replace(entitasSourceDir + dir("Serialization", "Configuration"), "Entitas.Serialization.Configuration/")
                .Replace(entitasSourceDir + "Serialization", "Entitas.Serialization")

                .Replace(entitasUnitySourceDir + "CodeGenerator", "Entitas.Unity.CodeGenerator")
                .Replace(entitasUnitySourceDir + "VisualDebugging", "Entitas.Unity.VisualDebugging")
                .Replace(entitasUnitySourceDir + dir("Serialization", "Blueprints"), "Entitas.Unity.Serialization.Blueprints/")
                .Replace(entitasUnitySourceDir + "Migration", "Entitas.Unity.Migration");

            if(file.Key.Contains(typeof(Entitas.Feature).Name) ||
                file.Key.Contains("BlueprintEntityExtension")) {
                expectedNamespace = "Entitas";
            } else {
                expectedNamespace = Regex.Match(fileName, expectedNamespacePattern)
                    .ToString()
                    .Replace("namespace ", string.Empty)
                    .Trim();
            }

            var foundNamespace = Regex.Match(file.Value, namespacePattern, RegexOptions.Multiline)
                .ToString()
                .Replace("namespace ", string.Empty)
                .Trim();

            each.Add(new NSpecTuple<string, string, string>(fileName, foundNamespace, expectedNamespace));
        }

        each.Do((fileName, given, expected) =>
            it["{0} namespace should be {2}".With(fileName, given, expected)] = () => given.should_be(expected)
        );
    }

    void when_checking_formatting() {
        var projectRoot = TestExtensions.GetProjectRoot();
        var sourceFiles = TestExtensions.GetSourceFiles(projectRoot);

        var each = new Each<string, string>();

        foreach(var file in sourceFiles) {
            each.Add(new NSpecTuple<string, string>(file.Key, file.Value));
        }

        each.Do((fileName, fileContent) => {
            it["{0} should not end with two newline".With(fileName, fileContent)] = () =>
                fileContent.EndsWith("\n\n", System.StringComparison.Ordinal).should_be_false();

            it["{0} should end with single newline".With(fileName, fileContent)] = () =>
                fileContent.EndsWith("\n", System.StringComparison.Ordinal).should_be_true();

            it["{0} should not contain new[]".With(fileName, fileContent)] = () =>
                fileContent.Contains("new[]").should_be_false();

            it["{0} should not contain if (".With(fileName, fileContent)] = () =>
                fileContent.Contains("if (").should_be_false();
        });
    }
}
