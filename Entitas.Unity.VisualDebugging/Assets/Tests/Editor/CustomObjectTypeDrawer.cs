using System;
using Entitas;
using Entitas.Unity.VisualDebugging;

public class CustomObjectTypeDrawer : ITypeDrawer, IDefaultInstanceCreator {
    public bool HandlesType(Type type) {
        return type == typeof(CustomObject);
    }

    public object CreateDefault(Type type) {
        return new CustomObject("Default");
    }

    public object DrawAndGetNewValue(Type type, string fieldName, object value, Entity entity, int index, IComponent component) {
        var myObject = (CustomObject)value;
        var fieldType = myObject.GetType().GetField("name").FieldType;
        EntityDebugEditor.DrawAndSetElement(fieldType, "customObject.name", myObject.name,
            entity, index, component, newValue => myObject.name = (string)newValue);
        return myObject;
    }
}
