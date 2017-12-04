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
            sourceFiles.Count.should_be_greater_than(150);
            sourceFiles.Count.should_be_less_than(250);
        };

        System.Console.WriteLine("sourceFiles: " + sourceFiles.Count);

        const string namespacePattern = @"(?:^namespace)\s.*\b";
        var expectedNamespacePattern = string.Format(@"[^\{0}]*", Path.DirectorySeparatorChar);

        var addonsDir = dir("Addons");

        var each = new Each<string, string, string>();

        foreach (var file in sourceFiles) {
            var fileName = file.Key
                               .Replace(dir(projectRoot), string.Empty)
                               .Replace(addonsDir, string.Empty);


            string expectedNamespace;
            expectedNamespace = Regex.Match(fileName, expectedNamespacePattern)
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
