using Entitas;

public class ComponentWithFields : IComponent {

    // Has one public field

    public string publicField;
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;
}
