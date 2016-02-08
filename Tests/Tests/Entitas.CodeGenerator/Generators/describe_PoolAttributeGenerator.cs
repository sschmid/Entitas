using System;
using System.Linq;
using Entitas.CodeGenerator;
using NSpec;

class describe_PoolAttributeGenerator : nspec {

    const bool logResults = false;

    void when_generating() {
        it["generates nothing input is empty"] = () => new PoolAttributeGenerator().Generate(new string[0]).Length.should_be(0);
        it["generates a PoolAttribute"] = () => {
            var files = new PoolAttributeGenerator().Generate(new [] { "MetaGame" });
            files.Length.should_be(1);
            files.Any(f => f.fileName == "MetaGameAttribute").should_be_true();
            var file = files.First(f => f.fileName == "MetaGameAttribute");
            const string expectedFileContent = @"public class MetaGameAttribute : Entitas.CodeGenerator.PoolAttribute {
    public MetaGameAttribute() : base(""MetaGame"") {
    }
}";

            #pragma warning disable
            if (logResults) {
                Console.WriteLine("should:\n'" + expectedFileContent + "'");
                Console.WriteLine("was:\n'" + file.fileContent + "'");
            }

            file.fileContent.should_be(expectedFileContent.ToUnixLineEndings());
        };


        it["generates multiple PoolAttributes"] = () => {
            var files = new PoolAttributeGenerator().Generate(new [] { "MetaGame", "UI" });
            files.Length.should_be(2);

            files.Any(f => f.fileName == "MetaGameAttribute").should_be_true();
            files.Any(f => f.fileName == "UIAttribute").should_be_true();

            var file = files.First(f => f.fileName == "UIAttribute");
            const string expectedFileContent = @"public class UIAttribute : Entitas.CodeGenerator.PoolAttribute {
    public UIAttribute() : base(""UI"") {
    }
}";
 
            #pragma warning disable
            if (logResults) {
                Console.WriteLine("should:\n'" + expectedFileContent + "'");
                Console.WriteLine("was:\n'" + file.fileContent + "'");
            }

            file.fileContent.should_be(expectedFileContent.ToUnixLineEndings());
        };
    }
}

