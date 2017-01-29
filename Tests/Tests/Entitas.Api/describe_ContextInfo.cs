using Entitas.Api;
using NSpec;

class describe_ContextInfo : nspec {

    void when_created() {

        it["sets fields with constructor values"] = () => {

            var componentNames = new[] { "Health", "Position", "View" };
            var componentTypes = new[] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) };
            const string contextName = "My Context";

            var info = new ContextInfo(contextName, componentNames, componentTypes);

            info.name.should_be(contextName);
            info.componentNames.should_be_same(componentNames);
            info.componentTypes.should_be_same(componentTypes);
        };
    }
}
