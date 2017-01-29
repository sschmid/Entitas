using System;
using Entitas;
using Entitas.Api;
using Entitas.Unity.VisualDebugging;

public class CustomObjectTypeDrawer : ITypeDrawer, IDefaultInstanceCreator {

    public bool HandlesType(Type type) {
        return type == typeof(CustomObject);
    }

    public object CreateDefault(Type type) {
        return new CustomObject("Default");
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, IEntity entity, int index, IComponent component) {
        var myObject = (CustomObject)value;
        var fieldType = myObject.GetType().GetField("name").FieldType;
        EntityDrawer.DrawAndSetElement(fieldType, "customObject.name", myObject.name,
            entity, index, component, (newComponent, newValue) => myObject.name = (string)newValue);
        return myObject;
    }
}
