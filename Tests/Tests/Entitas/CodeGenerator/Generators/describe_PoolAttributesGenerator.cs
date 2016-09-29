using NSpec;
using Entitas.CodeGenerator;
using System.Linq;

class describe_PoolAttributesGenerator : nspec {

    void when_generating() {
        it["generates nothing if input is empty"] = () => new PoolAttributesGenerator().Generate(new string[0]).Length.should_be(0);
        it["doesn't generate for default pool"] = () => new PoolAttributesGenerator().Generate(new [] { CodeGenerator.DEFAULT_POOL_NAME }).Length.should_be(0);

        it["generates a PoolAttribute"] = () => {
            var files = new PoolAttributesGenerator().Generate(new [] { "metaGame" });
            files.Length.should_be(1);
            var file = files[0];
            file.fileName.should_be("MetaGameAttribute");
            file.fileContent.should_be(@"using Entitas.CodeGenerator;

public class MetaGameAttribute : PoolAttribute {

    public MetaGameAttribute() : base(""MetaGame"") {
    }
}

".ToUnixLineEndings());
        };


        it["generates multiple PoolAttributes"] = () => {
            var files = new PoolAttributesGenerator().Generate(new [] { "MetaGame", "UI" });
            files.Length.should_be(2);

            files.Any(f => f.fileName == "MetaGameAttribute").should_be_true();
            files.Any(f => f.fileName == "UIAttribute").should_be_true();
            
            var file = files.First(f => f.fileName == "UIAttribute");
            file.fileContent.should_be(@"using Entitas.CodeGenerator;

public class UIAttribute : PoolAttribute {

    public UIAttribute() : base(""UI"") {
    }
}

".ToUnixLineEndings());
        };
    }
}
