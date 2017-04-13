using Entitas;
using Entitas.Utils;
using NSpec;

class describe_InterfaceTypeExtension : nspec {

    void when_type() {

        it["return false if type doesn't implement interface"] = () => {
            typeof(object).ImplementsInterface<IComponent>().should_be_false();
        };

        it["return false if type is same"] = () => {
            typeof(IComponent).ImplementsInterface<IComponent>().should_be_false();
        };

        it["return false if type is interface"] = () => {
            typeof(AnotherComponentInterface).ImplementsInterface<IComponent>().should_be_false();
        };

        it["return true if type implements interface"] = () => {
            typeof(StandardComponent).ImplementsInterface<IComponent>().should_be_true();
        };
    }
}
