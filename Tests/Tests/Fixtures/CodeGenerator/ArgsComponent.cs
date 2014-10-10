using Entitas;

public class ArgsComponent : IComponent {
    public int arg1;
    public string arg2;
    public static string extensions =
        @"using Entitas;

public static class ArgsComponentGeneratedExtension {

    public static void AddArgs(this Entity entity, int arg1, string arg2) {
        var component = new ArgsComponent();
        component.arg1 = arg1;
        component.arg2 = arg2;
        entity.AddComponent(ComponentIds.Args, component);
    }

    public static void ReplaceArgs(this Entity entity, ArgsComponent component) {
        entity.ReplaceComponent(ComponentIds.Args, component);
    }

    public static void ReplaceArgs(this Entity entity, int arg1, string arg2) {
        const int componentId = ComponentIds.Args;
        ArgsComponent component;
        if (entity.HasComponent(componentId)) {
            entity.WillRemoveComponent(componentId);
            component = (ArgsComponent)entity.GetComponent(componentId);
        } else {
            component = new ArgsComponent();
        }
        component.arg1 = arg1;
        component.arg2 = arg2;
        entity.ReplaceComponent(componentId, component);
    }

    public static bool HasArgs(this Entity entity) {
        return entity.HasComponent(ComponentIds.Args);
    }

    public static void RemoveArgs(this Entity entity) {
        entity.RemoveComponent(ComponentIds.Args);
    }

    public static ArgsComponent GetArgs(this Entity entity) {
        return (ArgsComponent)entity.GetComponent(ComponentIds.Args);
    }

}";
}
