using Entitas;
using System.Collections.Generic;

public class DictionaryComponent : IComponent {
    public Dictionary<string, int> arg;
    public static string extensions =
        @"using Entitas;

public static class DictionaryComponentGeneratedExtension {

    public static void AddDictionary(this Entity entity, System.Collections.Generic.Dictionary<string, int> arg) {
        var component = new DictionaryComponent();
        component.arg = arg;
        entity.AddComponent(ComponentIds.Dictionary, component);
    }

    public static void ReplaceDictionary(this Entity entity, DictionaryComponent component) {
        entity.ReplaceComponent(ComponentIds.Dictionary, component);
    }

    public static void ReplaceDictionary(this Entity entity, System.Collections.Generic.Dictionary<string, int> arg) {
        const int componentId = ComponentIds.Dictionary;
        DictionaryComponent component;
        if (entity.HasComponent(componentId)) {
            entity.WillRemoveComponent(componentId);
            component = (DictionaryComponent)entity.GetComponent(componentId);
        } else {
            component = new DictionaryComponent();
        }
        component.arg = arg;
        entity.ReplaceComponent(componentId, component);
    }

    public static bool HasDictionary(this Entity entity) {
        return entity.HasComponent(ComponentIds.Dictionary);
    }

    public static void RemoveDictionary(this Entity entity) {
        entity.RemoveComponent(ComponentIds.Dictionary);
    }

    public static DictionaryComponent GetDictionary(this Entity entity) {
        return (DictionaryComponent)entity.GetComponent(ComponentIds.Dictionary);
    }

}";
}

