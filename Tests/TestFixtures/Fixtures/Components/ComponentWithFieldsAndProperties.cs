using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test")]
public class ComponentWithFieldsAndProperties : IComponent
{
    // Has one public field
    public string publicField;

    // Has one public property
    public string publicProperty { get; set; }

#pragma warning disable

    // Should be ignored
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;

    // Should be ignored
    public static bool publicStaticProperty { get; set; }
    bool _privateProperty { get; set; }
    static bool _privateStaticProperty { get; set; }
}
