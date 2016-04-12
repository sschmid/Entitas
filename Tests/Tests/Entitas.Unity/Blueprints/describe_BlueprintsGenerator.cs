using Entitas;
using Entitas.CodeGenerator;
using NSpec;

class describe_BlueprintsGenerator : nspec {

    void when_generating() {

        it["doesn't create a Blueprints file if there aren't any blueprints"] = () => {
            var generator = new BlueprintsGenerator();
            var files = generator.Generate(new string[0]);
            files.Length.should_be(0);
        };

        it["adds getters for a blueprint to Blueprints ordered by name"] = () => {
            var generator = new BlueprintsGenerator();
            const string name1 = "MyBlueprint1";
            const string name2 = "MyBlueprint2";
            var blueprintNames = new [] { name2, name1 };

            var files = generator.Generate(blueprintNames);
            files.Length.should_be(1);

            var file = files[0];
            file.fileName.should_be("BlueprintsGeneratedExtension");
            file.fileContent.ToUnixLineEndings().should_be(@"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {
    public partial class Blueprints {
        public Blueprint MyBlueprint1 { get { return GetBlueprint(""MyBlueprint1""); } }
        public Blueprint MyBlueprint2 { get { return GetBlueprint(""MyBlueprint2""); } }
    }
}
");
        };
    }
}
