using System.IO;

namespace Entitas {

    public static class EntitasVersion {

        public static string GetVersion() {
            var assembly = typeof(Entity).Assembly;
            var stream = assembly.GetManifestResourceStream("version");
            var version = "";
            using(var reader = new StreamReader(stream)) {
                version = reader.ReadToEnd();
            }

            return version;
        }
    }
}
