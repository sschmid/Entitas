using System;
using System.Linq;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace Tests {

    class TestRunner {

        public static void Main(string[] args) {
            //format();

            //var tagOrClassName = "focus";
            var tagOrClassName = string.Empty;

            var types = typeof(TestRunner).Assembly.GetTypes();

            var finder = new SpecFinder(types, "");
            var tagsFilter = new Tags().Parse(tagOrClassName);
            var builder = new ContextBuilder(finder, tagsFilter, new DefaultConventions());
            var runner = new ContextRunner(tagsFilter, new ConsoleFormatter(), false);

            var results = runner.Run(builder.Contexts().Build());

            Environment.Exit(results.Failures().Count());
        }

        static void format() {
            var projectRoot = TestExtensions.GetProjectRoot();
            var sourceFiles = TestExtensions.GetSourceFilesInclAllProjects(projectRoot);

            foreach (var key in sourceFiles.Keys.ToArray()) {
                var fileContent = sourceFiles[key];

                if (key.Contains("TestRunner")) {
                    continue;
                }

                if (fileContent.EndsWith("\n\n", StringComparison.Ordinal)) {
                    fileContent = fileContent.Substring(0, fileContent.Length - 1);
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated double newline: " + key);
                }

                if (!fileContent.EndsWith("\n", StringComparison.Ordinal)) {
                    fileContent = fileContent + "\n";
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated no newline: " + key);
                }

                if (fileContent.Contains("\r\n")) {
                    fileContent = fileContent.Replace("\r\n", "\n");
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated \\r: " + key);
                }

                if (fileContent.Contains("new[]")) {
                    fileContent = fileContent.Replace("new[]", "new []");
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated new[]: " + key);
                }

                if (fileContent.Contains("if(")) {
                    fileContent = fileContent.Replace("if(", "if (");
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated if: " + key);
                }

                if (fileContent.Contains("foreach(")) {
                    fileContent = fileContent.Replace("foreach(", "foreach (");
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated foreach: " + key);
                }

                if (fileContent.Contains("for(")) {
                    fileContent = fileContent.Replace("for(", "for (");
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated for: " + key);
                }

                if (fileContent.Contains("switch(")) {
                    fileContent = fileContent.Replace("switch(", "switch (");
                    System.IO.File.WriteAllText(key, fileContent);
                    Console.WriteLine("Updated switch: " + key);
                }
            }
        }
    }
}
