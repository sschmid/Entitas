using NSpec;
using Entitas;

class describe_ContextInfo : nspec {

    void when_created() {

        it["has values"] = () => {

            var componentNames = new [] { "Health", "Position", "View" };
            var componentTypes = new [] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) };
            const string contextName = "My Context";

            var data = new ContextInfo(contextName, componentNames, componentTypes);

            data.name.should_be(contextName);
            data.componentNames.should_be_same(componentNames);
            data.componentTypes.should_be_same(componentTypes);
        };
    }
}
