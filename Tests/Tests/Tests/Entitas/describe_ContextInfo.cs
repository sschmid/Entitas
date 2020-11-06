using Entitas;
using NSpec;
using Shouldly;

class describe_ContextInfo : nspec {

    void when_created() {

        it["sets fields with constructor values"] = () => {

            var contextName = "My Context";
            var componentNames = new [] { "Health", "Position", "View" };
            var componentTypes = new [] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) };

            var info = new ContextInfo(contextName, componentNames, componentTypes);

            info.name.ShouldBe(contextName);
            info.componentNames.ShouldBeSameAs(componentNames);
            info.componentTypes.ShouldBeSameAs(componentTypes);
        };
    }
}
