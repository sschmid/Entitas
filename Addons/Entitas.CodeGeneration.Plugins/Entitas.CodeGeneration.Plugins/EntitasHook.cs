using System;
using System.Linq;
using DesperateDevs.Analytics;
using DesperateDevs.CodeGeneration.CodeGenerator;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EntitasHook : CodeGeneratorTrackingHook {

        protected override string name { get { return "entitas"; } }

        protected override TrackingData GetData() {
            var types = AppDomain.CurrentDomain.GetAllTypes();
            return new UserTrackingData {
                {
                    "x", "v:" + EntitasResources.GetVersion() +
                         ",e:" + (types.Any(type => type.FullName == "DesperateDevs.CodeGeneration.CodeGenerator.CLI.Program") ? "s" : "u") +
                         ",p:" + (types.Any(type => type.FullName == "DesperateDevs.Roslyn.CodeGeneration.Plugins.PluginUtil") ? "1" : "0") +
                         ",f:" + _files.Length +
                         ",cp:" + _files.Count(f => f.fileName.EndsWith("Component.cs", StringComparison.OrdinalIgnoreCase)) +
                         ",cx:" + _files.Count(f => f.fileName.EndsWith("Context.cs", StringComparison.OrdinalIgnoreCase)) +
                         ",l:" + _files.Select(file => file.fileContent.ToUnixLineEndings()).Sum(content => content.Split('\n').Length)
                }
            };
        }
    }
}
