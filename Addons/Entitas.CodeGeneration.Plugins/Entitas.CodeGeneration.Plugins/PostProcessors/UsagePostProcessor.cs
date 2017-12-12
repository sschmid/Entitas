using System;
using System.Linq;
using DesperateDevs.Analytics;
using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.Plugins;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class UsagePostProcessor : TrackingPostProcessor {

        public override string name { get { return "Usage"; } }
        public override int priority { get { return 9999; } }
        public override bool runInDryMode { get { return false; } }

        protected override string GetName() {
            return "entitas";
        }

        protected override TrackingData GetData(CodeGenFile[] files) {
            var d = new DesperateDevsTrackingData();
            d["x"] =
                "f:" + files.Length +
                ",cp:" + files.Count(f => f.fileName.EndsWith("Component.cs", StringComparison.OrdinalIgnoreCase)) +
                ",cx:" + files.Count(f => f.fileName.EndsWith("Context.cs", StringComparison.OrdinalIgnoreCase)) +
                ",p:" + (AppDomain.CurrentDomain.GetAllTypes().Any(type => type.FullName == "Entitas.Roslyn.CodeGeneration.Plugins.PluginUtil") ? "1" : "0") +
                ",e:" + (AppDomain.CurrentDomain.GetAllTypes().Any(type => type.FullName == "DesperateDevs.CodeGeneration.CodeGenerator.CLI.Program") ? "s" : "u");
            return d;
        }
    }
}
