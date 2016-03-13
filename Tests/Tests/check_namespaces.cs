using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NSpec;

class check_namespaces : nspec {

    static string getEntitasProjectDir() {
        var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        return dirInfo.FullName;
    }

    static string dir(params string[] paths) {
        return paths.Aggregate(string.Empty, (pathString, p) => pathString + p + Path.DirectorySeparatorChar);
    }

    static Dictionary<string, string> getSourceFiles(string path) {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains(dir("Generated")) &&
        !p.Contains(dir("Libraries")) &&
        !p.Contains(dir("Tests")) &&
        !p.Contains(dir("Examples")) &&
        !p.Contains(dir("Readme")) &&
        !p.Contains(dir("bin")) &&
        !p.Contains(dir("obj")) &&
        !p.Contains("AssemblyInfo.cs") &&
        !p.Contains("Commands.cs") &&
        !p.Contains("Program.cs")
        )
            .ToDictionary(p => p, p => File.ReadAllText(p));
    }

    void when_checking_namespaces() {
        var entitasProjectDir = getEntitasProjectDir();

        var entitasSourceDir = dir("Entitas", "Entitas");
        var entitasUnitySourceDir = dir("Entitas.Unity", "Assets", "Entitas", "Unity");

        var sourceFiles = getSourceFiles(entitasProjectDir);

        sourceFiles.Count.should_be_greater_than(80);
        sourceFiles.Count.should_be_less_than(150);

        const string namespacePattern = @"(?:^namespace)\s.*\b";
        string expectedNamespacePattern = string.Format(@"[^\{0}]*", Path.DirectorySeparatorChar);

        var each = new Each<string, string, string>();

        foreach (var file in sourceFiles) {

            string expectedNamespace;

            var fileName = file.Key
                .Replace(dir(entitasProjectDir), string.Empty)
                .Replace(entitasSourceDir + "CodeGenerator", "Entitas.CodeGenerator")
                .Replace(entitasSourceDir + "Serialization", "Entitas.Serialization")

                .Replace(entitasUnitySourceDir + "CodeGenerator", "Entitas.Unity.CodeGenerator")
                .Replace(entitasUnitySourceDir + "VisualDebugging", "Entitas.Unity.VisualDebugging")
                .Replace(entitasUnitySourceDir + "Migration", "Entitas.Unity.Migration");

            if (file.Key.Contains(typeof(Entitas.Feature).Name)) {
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
}

