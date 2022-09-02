using System;
using System.IO;
using System.Linq;

namespace Entitas.Migration {

    public static class MigrationUtils {

        public static MigrationFile[] GetFiles(string path, string searchPattern = "*.cs") {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                .Select(p => new MigrationFile(p, File.ReadAllText(p)))
                .ToArray();
        }

        public static void WriteFiles(MigrationFile[] files) {
            foreach (var file in files) {
                Console.WriteLine("Migrating: " + file.fileName);
                File.WriteAllText(file.fileName, file.fileContent);
            }
        }
    }
}
