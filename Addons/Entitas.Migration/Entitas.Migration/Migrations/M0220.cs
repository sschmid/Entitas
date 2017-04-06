using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration {

    public class M0220 : IMigration {

        public string version { get { return "0.22.0"; } }

        public string workingDirectory { get { return "where all systems are located"; } }

        public string description { get { return "Migrates IReactiveSystem to combine trigger and eventTypes to TriggerOnEvent"; } }

        const string TRIGGER_PATTERN = @"public\s*IMatcher\s*trigger\s*\{\s*get\s*\{\s*return\s*(?<matcher>.*?)\s*;\s*\}\s*\}";
        const string EVENT_TYPE_PATTERN = @"^\s*public\s*GroupEventType\s*eventType\s*\{\s*get\s*\{\s*return\s*GroupEventType\.(?<eventType>\w*)\s*;\s*\}\s*\}";

        const string TRIGGER_REPLACEMENT_FORMAT = @"public TriggerOnEvent trigger {{ get {{ return {0}.{1}(); }} }}";

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetFiles(path)
                .Where(file => Regex.IsMatch(file.fileContent, TRIGGER_PATTERN))
                .ToArray();

            for (int i = 0; i < files.Length; i++) {
                var file = files[i];

                var eventTypeMatch = Regex.Match(file.fileContent, EVENT_TYPE_PATTERN, RegexOptions.Multiline);
                var eventType = eventTypeMatch.Groups["eventType"].Value;
                file.fileContent = Regex.Replace(file.fileContent, EVENT_TYPE_PATTERN, string.Empty, RegexOptions.Multiline);

                file.fileContent = Regex.Replace(file.fileContent, TRIGGER_PATTERN,
                    match => string.Format(TRIGGER_REPLACEMENT_FORMAT, match.Groups["matcher"].Value, eventType),
                    RegexOptions.Multiline);
            }

            return files;
        }
    }
}
