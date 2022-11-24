using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1")]
public class ComponentWithFieldsAndProperties : IComponent
{
    // Has one public field
    public string PublicField;

    // Has one public property
    public string PublicProperty { get; set; }

#pragma warning disable

    // Should be ignored
    public static bool PublicStaticField;
    bool _privateField;
    static bool _privateStaticField;

    // Should be ignored
    public static bool PublicStaticProperty { get; set; }
    bool _privateProperty { get; set; }
    static bool _privateStaticProperty { get; set; }
}
