namespace Entitas
{
    /// Implement this interface if you want to create a component which
    /// you can add to an entity.
    /// Optionally, you can add these attributes:
    /// [Unique]: the code generator will generate additional methods for
    /// the context to ensure that only one entity with this component exists.
    /// E.g. context.isAnimating = true or context.SetResources();
    /// [MyContextName, MyOtherContextName]: You can make this component to be
    /// available only in the specified contexts.
    /// The code generator can generate these attributes for you.
    /// More available Attributes can be found in Entitas.CodeGeneration.Attributes/Attributes.
    public interface IComponent { }
}
