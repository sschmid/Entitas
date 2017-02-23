using NSpec;
using Entitas.CodeGenerator;

class describe_EntityIndexDataProvider : nspec {

    void when_providing() {

        it["creates data for each entity index"] = () => {
            var types = new [] { typeof(EntityIndexComponent), typeof(StandardComponent) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();

            data.Length.should_be(1);
            var d = ((EntityIndexData)data[0]);

            d.IsPrimary().GetType().should_be(typeof(bool));
            d.IsPrimary().should_be_false();

            d.GetEntityIndexType().GetType().should_be(typeof(string));
            d.GetEntityIndexType().should_be("EntityIndex");

            d.GetEntityIndexName().should_be("EntityIndex");
            d.GetEntityIndexName().GetType().should_be(typeof(string));

            d.GetContextNames().GetType().should_be(typeof(string[]));
            d.GetContextNames().Length.should_be(1);
            d.GetContextNames()[0].should_be("Game");

            d.GetKeyType().GetType().should_be(typeof(string));
            d.GetKeyType().should_be("string");

            d.GetComponentType().GetType().should_be(typeof(string));
            d.GetComponentType().should_be("EntityIndexComponent");

            d.GetMemberName().GetType().should_be(typeof(string));
            d.GetMemberName().should_be("value");
        };

        it["creates data for each primary entity index"] = () => {
            var types = new [] { typeof(PrimaryEntityIndexComponent), typeof(StandardComponent) };
            var provider = new EntityIndexDataProvider(types);
            var data = provider.GetData();

            data.Length.should_be(1);
            var d = ((EntityIndexData)data[0]);

            d.IsPrimary().should_be_true();
            d.GetEntityIndexType().should_be("PrimaryEntityIndex");
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
    }
}
