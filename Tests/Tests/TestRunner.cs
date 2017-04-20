using System;
using System.Linq;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace Tests {

    class TestRunner {

        public static void Main(string[] args) {
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
    }
}
