using Entitas.CodeGeneration.Plugins;
using NSpec;
using Entitas.Utils;
using Entitas.CodeGeneration;

class describe_UpdateCSProjPostProcessor : nspec {

    void when_post_processing() {

        xit["manual test"] = () => {
            var p = new UpdateCSProjPostProcessor();

            p.Configure(new Properties(
                "Entitas.CodeGeneration.Plugins.ProjectPath = " + TestExtensions.GetProjectRoot() + "/../Match-One/Assembly-CSharp.csproj" + "\n" + 
                "Entitas.CodeGeneration.Plugins.TargetDirectory = Assets/Sources"
            ));

            var files = new [] {
                new CodeGenFile("My/Generated/Folder/File1.cs", "Hello, world!", "Test"),
                new CodeGenFile("My/Generated/Folder/File2.cs", "Hello, world!", "Test")
            };

            p.PostProcess(files);
        };
    }
}
