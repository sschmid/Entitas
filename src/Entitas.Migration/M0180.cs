using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration
{
    public class M0180 : IMigration
    {
        public string Version => "0.18.0";
        public string WorkingDirectory => "where all systems are located";
        public string Description => "Migrates IReactiveSystem GetXyz methods to getters";

        const string MethodEndPattern = @"(\s|.)*?\}";
        const string TriggerPattern = @"public\s*IMatcher\s*GetTriggeringMatcher\s*\(\s*\)\s*\{\s*";
        const string TriggerEndPattern = TriggerPattern + MethodEndPattern;
        const string TriggerReplacement = "public IMatcher trigger { get { ";

        const string EventTypePattern = @"public\s*GroupEventType\s*GetEventType\s*\(\s*\)\s*\{\s*";
        const string EventTypePatternEnd = EventTypePattern + MethodEndPattern;
        const string EventTypeReplacement = "public GroupEventType eventType { get { ";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path)
            .Where(file => Regex.IsMatch(file.FileContent, TriggerPattern) || Regex.IsMatch(file.FileContent, EventTypePattern))
            .Select(file =>
                {
                    file.FileContent = Regex.Replace(file.FileContent, TriggerEndPattern, match => match.Value + " }", RegexOptions.Multiline);
                    file.FileContent = Regex.Replace(file.FileContent, EventTypePatternEnd, match => match.Value + " }", RegexOptions.Multiline);
                    file.FileContent = Regex.Replace(file.FileContent, TriggerPattern, TriggerReplacement, RegexOptions.Multiline);
                    file.FileContent = Regex.Replace(file.FileContent, EventTypePattern, EventTypeReplacement, RegexOptions.Multiline);
                    return file;
                }
            ).ToArray();
    }
}
