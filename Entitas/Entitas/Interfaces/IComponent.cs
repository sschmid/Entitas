namespace Entitas {

    /// Implement this interface if you want to create a component which you can add to an entity.
    /// Optionally, you can add these attributes:
    /// [SingleEntity]: the code generator will generate additional methods for the pool to ensure that only one entity with this component exists.
    /// E.g. pool.isAnimating = true or pool.SetResources();
    /// [MyPoolName, MyOtherPoolName]: You can make this component to be available only in the specified pools.
    /// The code generator can generate these attributes for you.
    /// More available Attributes can be found in CodeGenerator/Attributes.
    public interface IComponent {
    }
}

