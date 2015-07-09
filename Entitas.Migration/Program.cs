using System;
using System.Linq;

namespace Entitas.Migration {
    class MainClass {
        public static void Main(string[] args) {

            var allMigrations = new IMigration[] {
                new M0180()
            };

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
                Console.WriteLine(m.version + " - " + m.description);
            }
        }

        static void printVersionNotFound(string version, IMigration[] migrations) {
            Console.WriteLine("Could not find a migration for version '" + version + "'");
            printAllMigrations(migrations);
        }
    }
}
