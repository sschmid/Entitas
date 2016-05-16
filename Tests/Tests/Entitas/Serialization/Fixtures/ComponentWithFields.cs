using Entitas;
using Entitas.CodeGenerator;

#pragma warning disable
public class ComponentWithFields : IComponent {

    // Has one public field

    [IndexKey("myField")]
    public string publicField;
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;
}
