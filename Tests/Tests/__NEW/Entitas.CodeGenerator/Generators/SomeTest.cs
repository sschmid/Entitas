using NSpec;
using Entitas.CodeGenerator;

class SomeTest : nspec {

    [Tag("focus")]
    void when_generating() {


        it["prints code"] = () => {
            var types = new [] { typeof(MovableComponent) };
            var data = new ComponentDataProvider(types).GetData();
            var files = new MatcherGenerator().Generate(data);

            foreach(var file in files) {
                System.Console.WriteLine("file.fileContent: " + file.fileName + "\n" + file.fileContent);
            }
        };
    }
}
