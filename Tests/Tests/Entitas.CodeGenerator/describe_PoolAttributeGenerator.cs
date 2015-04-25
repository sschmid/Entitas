using NSpec;
using Entitas.CodeGenerator;

class describe_PoolAttributeGenerator : nspec {

    void when_generating() {
        it["generates nothing input is empty"] = () => PoolAttributeGenerator.GeneratePoolAttributes(new string[0]).Count.should_be(0);
        it["generates a PoolAttribute"] = () => {
            var attributes = PoolAttributeGenerator.GeneratePoolAttributes(new [] { "MetaGame" });
            attributes.Count.should_be(1);
            attributes.ContainsKey("MetaGameAttribute");
            attributes["MetaGameAttribute"].should_be(@"using Entitas.CodeGenerator;

public class MetaGameAttribute : PoolAttribute {
    public MetaGameAttribute() : base(""MetaGame"") {
    }
}

");
        };


        it["generates multiple PoolAttributes"] = () => {
            var attributes = PoolAttributeGenerator.GeneratePoolAttributes(new [] { "MetaGame", "UI" });
            attributes.Count.should_be(2);
            attributes.ContainsKey("MetaGameAttribute");
            attributes.ContainsKey("UIAttribute");
            attributes["UIAttribute"].should_be(@"using Entitas.CodeGenerator;

public class UIAttribute : PoolAttribute {
    public UIAttribute() : base(""UI"") {
    }
}

");
        };
    }
}

