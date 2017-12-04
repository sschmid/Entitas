using System;
using System.Linq;
using DesperateDevs.Utils;

namespace Entitas.Migration.CLI {

    class Program {

        public static void Main(string[] args) {
            var allMigrations = AppDomain.CurrentDomain
                                         .GetInstancesOf<IMigration>()
                                         .OrderBy(instance => instance.GetType().FullName)
                                         .ToArray();

            if (args == null) {
                printUsage(allMigrations);
            } else if (args.Length == 1) {
                var arg = args[0];
                if (arg == "-l") {
                    printAllMigrations(allMigrations);
                } else {
                    printUsage(allMigrations);
                }
            } else if (args.Length == 2) {
                var version = args[0];
                var path = args[1];
                var migrations = allMigrations.Where(m => m.version == version).ToArray();
                if (migrations.Length == 0) {
                    printVersionNotFound(version, allMigrations);
                } else {
                    foreach (var m in migrations) {
                        MigrationUtils.WriteFiles(m.Migrate(path));
                    }
                }
            } else {
                printUsage(allMigrations);
            }
        }

        static void printUsage(IMigration[] migrations) {
            Console.WriteLine(@"usage:
[-l]             - print all available versions
[version] [path] - apply migration of version [version] to source files located at [path]"
            );
        }

        static void printAllMigrations(IMigration[] migrations) {
            foreach (var m in migrations) {
                Console.WriteLine("========================================");
                Console.WriteLine(m.version + "\n  - " + m.description + "\n  - Use on folder, " + m.workingDirectory);
            }
            Console.WriteLine("========================================");
        }

        static void printVersionNotFound(string version, IMigration[] migrations) {
            Console.WriteLine("Could not find a migration for version '" + version + "'");
            printAllMigrations(migrations);
        }
    }
}
