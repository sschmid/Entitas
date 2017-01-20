using Entitas;
using Entitas.Api;

#pragma warning disable
public class ComponentWithFields : IComponent {

    // Has one public field

    [Some("myField")]
    public string publicField;
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;
}
