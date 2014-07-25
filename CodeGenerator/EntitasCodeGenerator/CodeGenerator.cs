using System.IO;

namespace Entitas.CodeGenerator {
    public class CodeGenerator {
        readonly static object _writeLock = new object();

        protected static void write(string path, string text) {
            lock (_writeLock) {
                using (StreamWriter writer = new StreamWriter(path, false)) {
                    writer.WriteLine(text);
                }
            }
        }
    }
}