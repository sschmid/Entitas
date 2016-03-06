using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NSpec;

class check_namespaces : nspec {
    static char _dirChar = Path.DirectorySeparatorChar;

    static string getEntitasProjectDir() {
        var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        return dirInfo.FullName;
    }

    static Dictionary<string, string> getSourceFiles(string path) {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains("Generated" + _dirChar) &&
                !p.Contains("Libraries" + _dirChar) &&
                !p.Contains("Tests" + _dirChar) &&
                !p.Contains("Examples" + _dirChar) &&
                !p.Contains("Readme" + _dirChar) &&
                !p.Contains("bin" + _dirChar) &&
                !p.Contains("obj" + _dirChar) &&
                !p.Contains("AssemblyInfo.cs") &&
                !p.Contains("Commands.cs") &&
                !p.Contains("Program.cs")
            )
            .ToDictionary(p => p, p => File.ReadAllText(p));
    }

    void when_checking_namespaces() {
        var entitasDir = getEntitasProjectDir();
        const string entitasUnity = "Entitas.Unity";
        var entitasUnityDir = entitasUnity + _dirChar + "Assets";

        var sourceFiles = getSourceFiles(entitasDir);

        sourceFiles.Count.should_be_greater_than(80);
        sourceFiles.Count.should_be_less_than(150);

        const string namespacePattern = @"(?:^namespace)\s.*\b";
        string expectedNamespacePattern = string.Format(@"[^\{0}]*", _dirChar);

        var each = new Each<string, string, string>();

        foreach (var file in sourceFiles) {

            var fileName = file.Key
                .Replace(entitasDir + _dirChar, string.Empty)
                .Replace(entitasUnityDir + _dirChar, string.Empty)

                .Replace("Entitas" + _dirChar + "Entitas" + _dirChar + "CodeGenerator", "Entitas.CodeGenerator")
                .Replace("Entitas" + _dirChar + "Entitas" + _dirChar + "Serialization", "Entitas.Serialization")

                .Replace("Entitas.Unity" + _dirChar + "CodeGenerator", "Entitas.Unity.CodeGenerator")
                .Replace("Entitas.Unity" + _dirChar + "VisualDebugging", "Entitas.Unity.VisualDebugging")
                .Replace("Entitas.Unity" + _dirChar + "Migration", "Entitas.Unity.Migration")
                .Replace("Entitas.Unity" + _dirChar + "Blueprints", "Entitas.Unity.Blueprints");

            var expectedNamespace = Regex.Match(fileName, expectedNamespacePattern)
                .ToString()
                .Replace("namespace ", string.Empty)
                .Trim();

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
}

