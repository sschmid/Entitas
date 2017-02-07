namespace Entitas {

    /// Inherit from this class if you want a component that is just a wrapper for some type.
    /// All attributes that work for normal components also work for components that inherit from this class.
    /// The body of the inheriting class should remain empty.
    /// Example use:
    /// <example>
    ///     // definition
    ///     public class NameComponent : WrapperComponent&lt;string&gt; { }
    ///     
    ///     // usage (after generator has run)
    ///     public string PhoneNumber(Entity e, Dictionary&lt;string, string&gt; phoneBook) {
    ///         if(e.Name == "Bob") { return "012 333 4545"; }
    ///
    ///         return phoneBook[e.Name];
    ///     }
    /// </example>
    public abstract class WrapperComponent<T> : IComponent {
        public T value;

        public static implicit operator T(WrapperComponent<T> component) { return component.value; }
    }
}
