using System;
using System.IO;
using System.Linq;

namespace Entitas.Migration {
    public static class MigrationUtils {
        public static MigrationFile[] GetSourceFiles(string path) {
            return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Select(p => new MigrationFile {
                    fileName = p,
                    fileContent = File.ReadAllText(p)
                }).ToArray();
        }

        public static void WriteFiles(MigrationFile[] files) {
            foreach (var file in files) {
                Console.WriteLine("Migrating: " + file.fileName);
                File.WriteAllText(file.fileName, file.fileContent);
            }
        }
    }
}

