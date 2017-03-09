using System.Reflection;
using Entitas.CodeGenerator;
using MyNamespace;
using NSpec;

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
            d.GetEntityIndexName().should_be("EntityIndex");

            d.GetContextNames().GetType().should_be(typeof(string[]));
            d.GetContextNames().Length.should_be(2);
            d.GetContextNames()[0].should_be("Test");
            d.GetContextNames()[1].should_be("Test2");

            d.GetKeyType().GetType().should_be(typeof(string));
            d.GetKeyType().should_be("string");

            d.GetComponentType().GetType().should_be(typeof(string));
            d.GetComponentType().should_be("EntityIndexComponent");

            d.GetComponentName().GetType().should_be(typeof(string));
            d.GetComponentName().should_be("EntityIndex");

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
            d.GetComponentName().should_be("PrimaryEntityIndex");
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
            methods.GetType().should_be(typeof(MethodInfo[]));
            methods.Length.should_be(2);
        };
        };
    }
}
