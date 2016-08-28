using Entitas.CodeGenerator;
using NSpec;

class describe_BlueprintsGenerator : nspec {

    static CodeGenFile[] generate(params string[] blueprints) {
        return new BlueprintsGenerator().Generate(blueprints);
    }

    static void generateFile(string expectedCode, params string[] blueprints) {
        expectedCode = expectedCode.ToUnixLineEndings();
        var files = new BlueprintsGenerator().Generate(blueprints);
        files[0].fileContent.should_be(expectedCode);
    }

    void when_generating() {

        it["doesn't create a Blueprints file if there aren't any blueprints"] = () => {
            generate(new string[0]).Length.should_be(0);
        };

        it["adds getters for a blueprint to Blueprints ordered by name"] = () => {
            var files = generate("MyBlueprint1", "MyBlueprint2");
            files.Length.should_be(1);

            var file = files[0];
            file.fileName.should_be("BlueprintsGeneratedExtension");
            file.fileContent.should_be(@"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {
    public partial class Blueprints {
        public Blueprint MyBlueprint1 { get { return GetBlueprint(""MyBlueprint1""); } }
        public Blueprint MyBlueprint2 { get { return GetBlueprint(""MyBlueprint2""); } }
    }
}
".ToUnixLineEndings());
        };

        it["removes whitespace for getter names"] = () => {
            generateFile(@"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {
    public partial class Blueprints {
        public Blueprint MyBlueprint { get { return GetBlueprint(""My Blueprint""); } }
    }
}
", "My Blueprint");
        };

        it["removes '-' for getter names"] = () => {
            generateFile(@"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {
    public partial class Blueprints {
        public Blueprint MyBlueprint { get { return GetBlueprint(""My-Blueprint""); } }
    }
}
", "My-Blueprint");
        };

        it["removes braces for getter names"] = () => {
            generateFile(@"using Entitas.Serialization.Blueprints;

namespace Entitas.Unity.Serialization.Blueprints {
    public partial class Blueprints {
        public Blueprint MyBlueprintVersion2 { get { return GetBlueprint(""MyBlueprint (Version 2)""); } }
    }
}
", "MyBlueprint (Version 2)");
        };
    }
}
