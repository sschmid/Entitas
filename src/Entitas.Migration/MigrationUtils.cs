using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Entitas.Migration
{
    public static class MigrationUtils
    {
        public static IEnumerable<MigrationFile> GetFiles(string path, string searchPattern = "*.cs") => Directory
            .GetFiles(path, searchPattern, SearchOption.AllDirectories)
            .Select(p => new MigrationFile(p, File.ReadAllText(p)));

        public static void WriteFiles(MigrationFile[] files)
        {
            foreach (var file in files)
            {
                Console.WriteLine($"Migrating: {file.FileName}");
                File.WriteAllText(file.FileName, file.FileContent);
            }
        }
    }
}
