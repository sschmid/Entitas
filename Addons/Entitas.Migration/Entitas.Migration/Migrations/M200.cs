using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.Migration
{
    public class M200 : IMigration
    {
        public string version
        {
            get { return "2.0.0"; }
        }

        public string workingDirectory
        {
            get { return "where source code files are located"; }
        }

        public string description
        {
            get { return "Updates code to use new generated extension methods"; }
        }

        public MigrationFile[] Migrate(string path)
        {
            var dataProvider = new ComponentDataProvider();
            dataProvider.objectCache = new Dictionary<string, object>();
            dataProvider.Configure(new Preferences("Jenny.properties", null));
            var data = dataProvider.GetData();

            return MigrationUtils.GetFiles(path)
                .Select(file => UpdateFile(file, data))
                .ToArray();
        }

        MigrationFile UpdateFile(MigrationFile file, CodeGeneratorData[] data)
        {
            var componentData = data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods());

            foreach (var d in componentData)
            {
                file.fileContent = d.GetMemberData().Length == 0
                    ? UpdateFlagApi(file, d)
                    : UpdateStandardApi(file, d);

                var upper = d.GetTypeName().ToComponentName().UppercaseFirst();

                foreach (var context in d.GetContextNames())
                {
                    // GameMatcher.Position
                    file.fileContent = file.fileContent.Replace(context + "Matcher." + upper, upper + "Matcher.Instance");
                }
            }

            return file;
        }

        string UpdateStandardApi(MigrationFile file, ComponentData data)
        {
            var upper = data.GetTypeName().ToComponentName().UppercaseFirst();
            var lower = data.GetTypeName().ToComponentName().LowercaseFirst();

            // e.position -> e.GetPosition()
            file.fileContent = file.fileContent.Replace("." + lower, ".Get" + upper + "()");

            // e.hasPosition -> e.HasPosition()
            file.fileContent = file.fileContent.Replace(".has" + upper, ".Has" + upper + "()");

            // context.scoreEntity -> context.GetScoreEntity()
            file.fileContent = file.fileContent.Replace("." + lower + "Entity", ".Get" + upper + "Entity()");

            return file.fileContent;
        }

        string UpdateFlagApi(MigrationFile file, ComponentData data)
        {
            // var x = e.isDestroyed
            var lowerPrefix = data.PrefixedComponentName().LowercaseFirst();

            file.fileContent = Regex.Replace(file.fileContent,
                @"=.*\." + lowerPrefix + @".*;",
                match => match.Value.Replace("." + data.PrefixedComponentName() + "()"));

            // e.isDestroyed = x;
            file.fileContent = Regex.Replace(file.fileContent,
                @"\." + lowerPrefix + @".*=\s*(.*);",
                match => "." + data.PrefixedComponentName() + "(" + match.Groups[1] + ");");

            return file.fileContent;
        }
    }
}
