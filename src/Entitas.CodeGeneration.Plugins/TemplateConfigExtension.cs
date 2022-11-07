using System.IO;
using System.Linq;

namespace Entitas.CodeGeneration.Plugins
{
    public static class TemplateConfigExtension
    {
        public static string FindTemplate(this TemplatesConfig config, string fileName)
        {
            foreach (var dir in config.templates)
            {
                var template = Directory
                    .GetFiles(dir, fileName, SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (template != null)
                    return template;
            }

            return null;
        }

        public static string FindComponentContextTemplate(this TemplatesConfig config) => config.FindTemplate("ComponentContext.txt");
        public static string FindFlagComponentContextTemplate(this TemplatesConfig config) => config.FindTemplate("FlagComponentContext.txt");
        public static string FindMethodArgumentTemplate(this TemplatesConfig config) => config.FindTemplate("MethodArgument.txt");
        public static string FindMethodParameterTemplate(this TemplatesConfig config) => config.FindTemplate("MethodParameter.txt");
    }
}
