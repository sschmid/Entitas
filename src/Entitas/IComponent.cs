namespace Entitas
{
    /// Implement this interface if you want to create a component which
    /// you can add to an entity.
    /// Mandatory attributes from Entitas.Generators.Attributes:
    /// [Context(typeof(MainContext))]: Use the context attribute to make this
    /// component available in the specified context.
    /// Optionally, you can add these attributes:
    /// [Unique]: the code generator will generate additional methods for
    /// the context to ensure that only one entity with this component exists.
    /// E.g. context.AddLoading() or context.SetPlayerName(name);
    /// More available attributes can be found in Entitas.Generators.Attributes.
    public interface IComponent { }
}
