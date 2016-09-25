using System;
using System.Linq;
using System.Reflection;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace Tests {

    class TestRunner {

        public static void Main(string[] args) {

            format();

            //var tagOrClassName = "focus";
            var tagOrClassName = string.Empty;

            var types = Assembly.GetAssembly(typeof(describe_Entity)).GetTypes();

            var finder = new SpecFinder(types, "");
            var tagsFilter = new Tags().Parse(tagOrClassName);
            var builder = new ContextBuilder(finder, tagsFilter, new DefaultConventions());
            var runner = new ContextRunner(tagsFilter, new ConsoleFormatter(), false);

            var results = runner.Run(builder.Contexts().Build());

            Environment.Exit(results.Failures().Count());
        }

        static void format() {
            var projectRoot = TestExtensions.GetProjectRoot();
            var sourceFiles = TestExtensions.GetSourceFiles(projectRoot);


            foreach(var key in sourceFiles.Keys.ToArray()) {
                var fileContent = sourceFiles[key];

                //if(fileContent.EndsWith("\n\n", StringComparison.Ordinal)) {
                //    fileContent = fileContent.Substring(0, fileContent.Length - 1);
                //    System.IO.File.WriteAllText(key, fileContent);
                //    Console.WriteLine("Updated " + key);
                //}

                //if(!fileContent.EndsWith("\n", StringComparison.Ordinal)) {
                //    fileContent = fileContent + "\n";
                //    System.IO.File.WriteAllText(key, fileContent);
                //    Console.WriteLine("Updated " + key);
                //}

                //if(fileContent.Contains("new[]")) {
                //    fileContent = fileContent.Replace("new[]", "new []");
                //    System.IO.File.WriteAllText(key, fileContent);
                //    Console.WriteLine("Updated " + key);
                //}

                //if(fileContent.Contains("if (")) {
                //    fileContent = fileContent.Replace("if (", "if(");
                //    System.IO.File.WriteAllText(key, fileContent);
                //    Console.WriteLine("Updated " + key);
                //}

                //if(fileContent.Contains("foreach (")) {
                //    fileContent = fileContent.Replace("foreach (", "foreach(");
                //    System.IO.File.WriteAllText(key, fileContent);
                //    Console.WriteLine("Updated " + key);
                //}
            }
        }
    }
}
