using Entitas.CodeGeneration.Plugins;
using My.Namespace;
using MyNamespace;
using NSpec;
using Entitas.Utils;

class describe_EntityIndexDataProvider : nspec {

    void when_providing() {

        it["creates data for each entity index"] = () => {
            var types = new [] { typeof(EntityIndexComponent), typeof(StandardComponent) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();

            data.Length.should_be(1);
            var d = ((EntityIndexData)data[0]);

            d.GetEntityIndexType().GetType().should_be(typeof(string));
            d.GetEntityIndexType().should_be("Entitas.EntityIndex");

            d.IsCustom().GetType().should_be(typeof(bool));
            d.IsCustom().should_be_false();

            d.GetEntityIndexName().GetType().should_be(typeof(string));
            d.GetEntityIndexName().should_be("MyNamespaceEntityIndex");

            d.GetContextNames().GetType().should_be(typeof(string[]));
            d.GetContextNames().Length.should_be(2);
            d.GetContextNames()[0].should_be("Test");
            d.GetContextNames()[1].should_be("Test2");

            d.GetKeyType().GetType().should_be(typeof(string));
            d.GetKeyType().should_be("string");

            d.GetComponentType().GetType().should_be(typeof(string));
            d.GetComponentType().should_be("My.Namespace.EntityIndexComponent");

            d.GetMemberName().GetType().should_be(typeof(string));
            d.GetMemberName().should_be("value");
        };

        it["creates data for each primary entity index"] = () => {
            var types = new [] { typeof(PrimaryEntityIndexComponent), typeof(StandardComponent) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();

            data.Length.should_be(1);
            var d = ((EntityIndexData)data[0]);

            d.GetEntityIndexType().should_be("Entitas.PrimaryEntityIndex");
            d.IsCustom().should_be_false();
            d.GetEntityIndexName().should_be("PrimaryEntityIndex");
            d.GetContextNames().Length.should_be(1);
            d.GetContextNames()[0].should_be("Game");
            d.GetKeyType().should_be("string");
            d.GetComponentType().should_be("PrimaryEntityIndexComponent");
            d.GetMemberName().should_be("value");
        };

        it["ignores abstract components"] = () => {
            var types = new [] { typeof(AbstractEntityIndexComponent) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();
            data.Length.should_be(0);
        };

        it["creates data for custom entity index class"] = () => {
            var types = new [] { typeof(CustomEntityIndex) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();

            data.Length.should_be(1);
            var d = ((EntityIndexData)data[0]);

            d.GetEntityIndexType().should_be("MyNamespace.CustomEntityIndex");
            d.IsCustom().should_be_true();
            d.GetEntityIndexName().should_be("MyNamespaceCustomEntityIndex");
            d.GetContextNames().Length.should_be(1);
            d.GetContextNames()[0].should_be("Test");

            var methods = d.GetCustomMethods();
            methods.GetType().should_be(typeof(MethodData[]));
            methods.Length.should_be(2);
        };

        it["ignores non IComponent"] = () => {
            var types = new [] { typeof(ClassWithEntitIndexAttribute) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();

            data.Length.should_be(0);
        };

        it["configure"] = () => {

            Properties properties = null;

            before = () => {
                properties = new Properties(
                                    "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n" +
                                    "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = true"
                                );
            };

            it["ignores namespaces"] = () => {
                var types = new [] { typeof(EntityIndexComponent), typeof(StandardComponent) };
                var provider = new EntityIndexDataProvider(types);
                provider.Configure(properties);
                var data = provider.GetData();

                data.Length.should_be(1);
                var d = ((EntityIndexData)data[0]);

                d.GetEntityIndexName().should_be("EntityIndex");

            };
            it["gets default context"] = () => {
                var types = new [] { typeof(EntityIndexNoContextComponent), typeof(StandardComponent) };
                var provider = new EntityIndexDataProvider(types);
                provider.Configure(properties);
                var data = provider.GetData();

                data.Length.should_be(1);
                var d = ((EntityIndexData)data[0]);

                d.GetContextNames().Length.should_be(1);
                d.GetContextNames()[0].should_be("ConfiguredContext");
            };
        };
    }
}
